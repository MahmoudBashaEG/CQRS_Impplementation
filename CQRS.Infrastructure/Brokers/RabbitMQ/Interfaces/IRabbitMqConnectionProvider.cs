using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace CQRS.Infrastructure.Brokers.RabbitMQ.Interfaces
{
    public interface IRabbitMqConnectionProvider
    {
        Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken = default);
        Task<IChannel> CreateChannelAsync(CancellationToken cancellationToken = default);
    }
}
