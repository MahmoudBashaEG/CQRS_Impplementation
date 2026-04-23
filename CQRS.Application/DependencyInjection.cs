using CQRS.Application.BackgroundServices;
using CQRS.Application.Common.Behaviors;
using CQRS.Application.Interfaces.Brokers;
using CQRS.Application.Services.Brokers;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CQRS.Application
{
    public static class DependencyInjection
    {
        public async static Task<IServiceCollection> AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            });

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            await AddRabbitMQ(services);

            services.AddSingleton<IBrokerProducer, RabbitMQProducerBroker>();
            services.AddHostedService<RabbitMQEventConsumerService>();

            return services;
        }


        private async static Task AddRabbitMQ(this IServiceCollection services)
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
            };

            var connection = await factory.CreateConnectionAsync();
            services.AddSingleton<IConnection>(connection);
        }
    }
}
