using CQRS.Application.Interfaces.Brokers;
using CQRS.Domain.EntityDomains;
using CQRS.Domain.Enums;
using CQRS.Infrastructure.Brokers.RabbitMQ.Interfaces;
using CQRS.Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace CQRS.Infrastructure.Brokers.RabbitMQ
{
    public class RabbitMQProducer : IBrokerProducer, IAsyncDisposable
    {
        private readonly IRabbitMqConnectionProvider _connectionProvider;

        private IChannel? _channel;
        private readonly SemaphoreSlim _publishLock = new(1, 1);

        private readonly ScalingConfigurations _scalingConfigurations;
        private readonly RabbitMqOptions _options;

        public RabbitMQProducer(
            IOptions<ScalingConfigurations> scalingConfigurations,
            IOptions<RabbitMqOptions> options,
            IRabbitMqConnectionProvider connectionProvider)
        {
            _scalingConfigurations = scalingConfigurations.Value;
            _options = options.Value;
            _connectionProvider = connectionProvider;
        }

        public async Task PublishAsync(OutboxMessage message, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (_scalingConfigurations.IsReadingAndWritingTheSameDatabase)
                return;

            _channel ??= await _connectionProvider.CreateChannelAsync(cancellationToken);
            if (_channel is null)
                throw new InvalidOperationException("RabbitMQ channel was not created.");

            await _publishLock.WaitAsync(cancellationToken);

            try
            {
                await _channel.ExchangeDeclareAsync(
                    exchange: _options.ExchangeName,
                    type: ExchangeType.Topic,
                    durable: true,
                    autoDelete: false);

                var routingKey = RabbitMQEventRoutingKeyMapper.ToRoutingKey(message.EventType);

                var body = Encoding.UTF8.GetBytes(message.Payload);
                var properties = new BasicProperties
                {
                    Persistent = true,
                    MessageId = message.Id.ToString(),
                    ContentType = "application/json",
                    Type = message.EventType.ToString(),
                    Timestamp = new AmqpTimestamp(
                        new DateTimeOffset(message.OccurredOnUtc).ToUnixTimeSeconds())
                };

                await _channel.BasicPublishAsync(
                    exchange: _options.ExchangeName,
                    routingKey: routingKey,
                    mandatory: true,
                    basicProperties: properties,
                    body: body);
            }
            finally
            {
                _publishLock.Release();
            }

        }

        public async ValueTask DisposeAsync()
        {
            if (_channel is not null)
            {
                await _channel.DisposeAsync();
            }
            _publishLock.Dispose();
        }
    }

}
