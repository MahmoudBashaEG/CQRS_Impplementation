using CQRS.Application.Events.BrokerEvents;
using CQRS.Application.Interfaces.Infrastructure;
using CQRS.CrossCutting.Configurations;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CQRS.Application.Interfaces.Brokers
{
    public interface IBrokerEventHandler
    {
        public BrokerEventsEnum BrokerEventType { get; }
        public Task Handle(IServiceScopeFactory serviceScopeFactory, ScalingConfigurations scalingConfigurations, string jsonMessage);
    }
}
