using CQRS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace CQRS.Domain.EntityDomains
{
    public sealed class OutboxMessage
    {
        public Guid Id { get; set; }

        public BrokerEventsEnum EventType { get; set; }

        public string Payload { get; set; } = null!;

        public DateTime OccurredOnUtc { get; set; }

        public DateTime? ProcessedOnUtc { get; set; }

        public string? Error { get; set; }

        public int RetryCount { get; set; }

        public static OutboxMessage BuildOutboxMessage<T>(BrokerEventsEnum eventType, T payload)
        {
            return new OutboxMessage
            {
                Id = Guid.NewGuid(),
                EventType = eventType,
                Payload = JsonSerializer.Serialize<T>(payload),
                OccurredOnUtc = DateTime.UtcNow,
                RetryCount = 0,
                ProcessedOnUtc = null,
                Error = null,
            };
        }
    }
}
