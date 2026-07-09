using CQRS.Application.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace CQRS.Application.Interfaces.Brokers
{
    public interface IBrokerConsumer
    {
        Task StartConsumingAsync(Func<BrokerMessageEnvelope, CancellationToken, Task> onMessageAsync, CancellationToken cancellationToken);
    }
}
