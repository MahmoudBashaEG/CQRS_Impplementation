using CQRS.Application.Events;
using CQRS.Application.Interfaces.Infrastructure;
using CQRS.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CQRS.Application.Interfaces.Brokers
{
    public interface IBrokerEventHandler
    {
        public BrokerEventsEnum EventType { get; }
        public Task HandleAsync(BrokerMessageEnvelope message, CancellationToken cancellationToken);
    }
}
