using CQRS.Application.Interfaces.Brokers;
using CQRS.Application.Interfaces.Infrastructure;
using CQRS.Infrastructure.BackgroundServices;
using CQRS.Infrastructure.Brokers;
using CQRS.Infrastructure.Brokers.RabbitMQ;
using CQRS.Infrastructure.Brokers.RabbitMQ.Interfaces;
using CQRS.Infrastructure.Persistence.ReadDb;
using CQRS.Infrastructure.Persistence.WriteDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace CQRS.Infrastructure
{
    public static class DependencyInjection
    {
        public async static Task<IServiceCollection> AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            /*            //Commands & Query points to different database
                        services.AddDbContext<ApplicationWriteDbContext>(options =>
                            options.UseSqlServer(configuration.GetConnectionString("Both"), x => x.MigrationsAssembly(typeof(ApplicationWriteDbContext).Assembly.FullName)));

                        services.AddScoped<IWriteDbContext>(provider => provider.GetRequiredService<ApplicationWriteDbContext>());

                        services.AddDbContext<ApplicationReadDbContext>(options =>
                            options.UseSqlServer(configuration.GetConnectionString("Both"))
                            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

                        services.AddScoped<IReadDbContext>(provider => provider.GetRequiredService<ApplicationReadDbContext>());*/




            // Commands & Query points to different database
            services.AddDbContext<WriteDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("Write"), x => x.MigrationsAssembly(typeof(WriteDbContext).Assembly.FullName)));

            services.AddScoped<IWriteDbContext>(provider => provider.GetRequiredService<WriteDbContext>());

            services.AddDbContext<ReadDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("Read"), x => x.MigrationsAssembly(typeof(ReadDbContext).Assembly.FullName))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

            services.AddScoped<IReadDbContext>(provider => provider.GetRequiredService<ReadDbContext>());

            await AddRabbitMQ(services);
            await AddBackgroundServices(services);

            return services;
        }

        private static async Task AddBackgroundServices(IServiceCollection services)
        {
            services.AddHostedService<OutboxPublisherBackgroundService>();
            services.AddHostedService<OutboxConsumerBackgroundService>();
        }

        private async static Task AddRabbitMQ(this IServiceCollection services)
        {
            services.AddSingleton<IRabbitMqConnectionProvider, RabbitMqConnectionProvider>();

            services.AddSingleton<IBrokerProducer, RabbitMQProducer>();
            services.AddSingleton<IBrokerConsumer, RabbitMQConsumer>();
        }

    }
}
