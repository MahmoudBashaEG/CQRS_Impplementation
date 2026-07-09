using CQRS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CQRS.Application.Events
{
    public class BrokerMessageEnvelope
    {
        public Guid MessageId { get; init; }

        public BrokerEventsEnum EventType { get; init; }

        public string Payload { get; init; } = null!;

        public DateTime OccurredOnUtc { get; init; }

        public string RoutingKey { get; init; } = null!;
    }
}
