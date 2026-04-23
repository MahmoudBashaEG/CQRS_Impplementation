using CQRS.Application.Events.BrokerEvents;
using System;
using System.Collections.Generic;
using System.Text;

namespace CQRS.Application.Interfaces.Brokers
{
    public interface IBrokerProducer
    {
        Task PublishAsync<T>(T message, BrokerEventsEnum eventType);
    }
}
