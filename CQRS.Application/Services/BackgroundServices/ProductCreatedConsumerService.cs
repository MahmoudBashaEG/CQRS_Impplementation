using CQRS.Application.Interfaces.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CQRS.Application.BackgroundServices
{
    internal class RabbitMQEventConsumerService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public RabbitMQEventConsumerService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {


            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
