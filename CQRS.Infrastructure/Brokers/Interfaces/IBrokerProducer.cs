using CQRS.Domain.EntityDomains;
using CQRS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CQRS.Application.Interfaces.Brokers
{
    public interface IBrokerProducer
    {
        Task PublishAsync(OutboxMessage message, CancellationToken cancellationToken);
    }
}
