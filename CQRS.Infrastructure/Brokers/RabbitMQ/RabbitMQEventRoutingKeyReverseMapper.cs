using CQRS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CQRS.Infrastructure.Brokers.RabbitMQ
{
    public static class RabbitMQEventRoutingKeyReverseMapper
    {
        public static BrokerEventsEnum ToEventType(string routingKey)
        {
            return routingKey switch
            {
                "product.created.v1" => BrokerEventsEnum.ProductCreatedEvent,

                _ => throw new ArgumentOutOfRangeException(
                    nameof(routingKey),
                    routingKey,
                    "Unsupported RabbitMQ routing key.")
            };
        }
    }
}
