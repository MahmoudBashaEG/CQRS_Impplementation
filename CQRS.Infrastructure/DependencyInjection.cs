using CQRS.Application.Interfaces.Infrastructure;
using CQRS.Infrastructure.ApplicationReadDb;
using CQRS.Infrastructure.ApplicationWriteDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace CQRS.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
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
            services.AddDbContext<ApplicationWriteDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("Write"), x => x.MigrationsAssembly(typeof(ApplicationWriteDbContext).Assembly.FullName)));

            services.AddScoped<IWriteDbContext>(provider => provider.GetRequiredService<ApplicationWriteDbContext>());

            services.AddDbContext<ApplicationReadDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("Read"), x => x.MigrationsAssembly(typeof(ApplicationReadDbContext).Assembly.FullName))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

            services.AddScoped<IReadDbContext>(provider => provider.GetRequiredService<ApplicationReadDbContext>());


            return services;
        }
    }
}
