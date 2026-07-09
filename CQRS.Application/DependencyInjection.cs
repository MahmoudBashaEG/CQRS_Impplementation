using CQRS.Application.Common.Behaviors;
using CQRS.Application.Interfaces.Brokers;
using CQRS.Application.Services.BrokerEventHandlers;
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

            services.AddScoped<IBrokerEventHandler, ProductCreatedEventHandler>();


            return services;
        }


    }
}
