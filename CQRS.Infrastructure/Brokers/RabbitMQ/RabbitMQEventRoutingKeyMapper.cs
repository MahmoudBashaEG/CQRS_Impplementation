using CQRS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CQRS.Infrastructure.Brokers.RabbitMQ
{
    public static class RabbitMQEventRoutingKeyMapper
    {
        public static string ToRoutingKey(BrokerEventsEnum eventType)
        {
            return eventType switch
            {
                BrokerEventsEnum.ProductCreatedEvent => "product.created.v1",

                _ => throw new ArgumentOutOfRangeException(
                    nameof(eventType),
                    eventType,
                    "Unsupported broker event type")
            };
        }
    }
}
