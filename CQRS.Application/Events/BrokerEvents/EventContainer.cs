using CQRS.Domain.EntityDomains;
using System;
using System.Collections.Generic;
using System.Text;

namespace CQRS.Application.Events.BrokerEvents
{
    public class EventContainer<T>
    {
        public BrokerEventsEnum EventType { get; set; }
        public string MessageJson { get; set; }
        public DateTime OccuredOnUTC { get; private set; } = DateTime.UtcNow;

    }
}
