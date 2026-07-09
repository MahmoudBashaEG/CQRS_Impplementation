using CQRS.Application.Events;
using CQRS.Application.Interfaces.Brokers;
using CQRS.Domain.Enums;
using CQRS.Infrastructure.Brokers.RabbitMQ.Interfaces;
using CQRS.Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace CQRS.Infrastructure.Brokers.RabbitMQ
{
    public class RabbitMQConsumer : IBrokerConsumer, IAsyncDisposable
    {
        private readonly IRabbitMqConnectionProvider _connectionProvider;
        private readonly ScalingConfigurations _scalingConfigurations;
        private readonly RabbitMqOptions _options;

        private IChannel? _channel;


        public RabbitMQConsumer(
            IOptions<ScalingConfigurations> scalingConfigurations,
            IOptions<RabbitMqOptions> options,
            IRabbitMqConnectionProvider connectionProvider)
        {
            _scalingConfigurations = scalingConfigurations.Value;
            _options = options.Value;
            _connectionProvider = connectionProvider;
        }

        public async Task StartConsumingAsync(Func<BrokerMessageEnvelope, CancellationToken, Task> onMessageAsync, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (_scalingConfigurations.IsReadingAndWritingTheSameDatabase)
                return;

            _channel ??= await _connectionProvider.CreateChannelAsync(cancellationToken);
            if (_channel is null)
                throw new InvalidOperationException("RabbitMQ channel was not created.");

            await DeclareTopologyAsync(cancellationToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (_, eventArgs) =>
            {
                await ProcessRabbitMqMessageAsync(
                    eventArgs,
                    onMessageAsync,
                    cancellationToken);
            };

            await _channel.BasicConsumeAsync(
                queue: _options.QueueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: cancellationToken);

            await Task.Delay(Timeout.Infinite, cancellationToken);
        }

        private async Task DeclareTopologyAsync(CancellationToken cancellationToken)
        {
            if (_channel is null)
                throw new InvalidOperationException("RabbitMQ channel was not created.");

            await _channel.ExchangeDeclareAsync(
                exchange: _options.ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                cancellationToken: cancellationToken);

            await _channel.QueueDeclareAsync(
                queue: _options.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken);

            await _channel.QueueBindAsync(
                queue: _options.QueueName,
                exchange: _options.ExchangeName,
                routingKey: "#",
                cancellationToken: cancellationToken);

            await _channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: 1,
                global: false,
                cancellationToken: cancellationToken);
        }

        private async Task ProcessRabbitMqMessageAsync(
            BasicDeliverEventArgs eventArgs,Func<BrokerMessageEnvelope, CancellationToken, Task> onMessageAsync, CancellationToken cancellationToken)
        {
            if (_channel is null)
                throw new InvalidOperationException("RabbitMQ channel was not created.");

            try
            {
                var message = MapToEnvelope(eventArgs);

                await onMessageAsync(message, cancellationToken);

                await _channel.BasicAckAsync(
                    deliveryTag: eventArgs.DeliveryTag,
                    multiple: false,
                    cancellationToken: cancellationToken);
            }
            catch
            {
                await _channel.BasicNackAsync(
                    deliveryTag: eventArgs.DeliveryTag,
                    multiple: false,
                    requeue: true,
                    cancellationToken: cancellationToken);
            }
        }

        private static BrokerMessageEnvelope MapToEnvelope(BasicDeliverEventArgs eventArgs)
        {
            var payload = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

            var properties = eventArgs.BasicProperties;

            if (!Guid.TryParse(properties.MessageId, out var messageId))
            {
                throw new InvalidOperationException(
                    $"Invalid or missing RabbitMQ MessageId: {properties.MessageId}");
            }

            var isEnumVal = Enum.TryParse<BrokerEventsEnum>(
                    properties.Type,
                    ignoreCase: true,
                    out var parsedEventType);

            var eventType = !string.IsNullOrWhiteSpace(properties.Type) && isEnumVal
                    ? parsedEventType
                    : RabbitMQEventRoutingKeyReverseMapper.ToEventType(eventArgs.RoutingKey);

            var occurredOnUtc = properties.Timestamp.UnixTime > 0
                ? DateTimeOffset
                    .FromUnixTimeSeconds(properties.Timestamp.UnixTime)
                    .UtcDateTime
                : DateTime.UtcNow;

            return new BrokerMessageEnvelope
            {
                MessageId = messageId,
                EventType = eventType,
                Payload = payload,
                OccurredOnUtc = occurredOnUtc,
                RoutingKey = eventArgs.RoutingKey
            };
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel is not null)
                await _channel.DisposeAsync();
        }
    }
}
