using System;
using System.Collections.Generic;
using System.Text;

namespace CQRS.Infrastructure.Configurations
{
    public class RabbitMqOptions
    {
        public string HostName { get; init; }

        public int Port { get; init; }

        public string UserName { get; init; }

        public string Password { get; init; }

        public string ExchangeName { get; init; }
        public string QueueName { get; init; }
    }
}
