using CQRS.Application.Events.BrokerEvents;
using CQRS.Application.Interfaces.Brokers;
using CQRS.CrossCutting.Configurations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace CQRS.Application.Services.Brokers
{
    public class RabbitMQProducerBroker : IBrokerProducer
    {
        private readonly IConnection _connection;
        private readonly ScalingConfigurations _scalingConfigurations;
        public RabbitMQProducerBroker(
            IConnection connection,
            IOptions<ScalingConfigurations> scalingConfigurations
            )
        {
            _connection = connection;
            _scalingConfigurations = scalingConfigurations.Value;
        }

        public async Task PublishAsync<T>(T message, BrokerEventsEnum eventType)
        {
            if (_scalingConfigurations.IsReadingAndWritingTheSameDatabase)
                return;

            var eventContainer = new EventContainer<T>
            {
                MessageJson = JsonSerializer.Serialize(message),
                EventType = eventType,
            };

            using var channel = await _connection.CreateChannelAsync();

            var json = JsonSerializer.Serialize(eventContainer);
            var body = Encoding.UTF8.GetBytes(json);

            await channel.BasicPublishAsync(
                exchange: "ex.fanout.messages",
                routingKey: string.Empty,
                body: body);
        }
    }
}
