using CQRS.Application.Interfaces.Infrastructure;
using CQRS.Domain.EntityDomains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace CQRS.Infrastructure.Persistence.WriteDb
{
    // Add-Migration InitialWriteDb -Context WriteDbContext -Project CQRS.Infrastructure -StartupProject CQRS.API -OutputDir Persistence\WriteDb\Migrations
    // Update-Database -Context WriteDbContext -Project CQRS.Infrastructure -StartupProject CQRS.API

    public class WriteDbContext : DbContext, IWriteDbContext
    {

        public WriteDbContext(DbContextOptions<WriteDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OutboxMessage>(builder =>
            {
                builder.HasKey(x => x.Id);

                builder.Property(x => x.EventType)
                    .IsRequired();

                builder.Property(x => x.Payload)
                    .IsRequired();

                builder.Property(x => x.OccurredOnUtc)
                    .IsRequired();

                builder.Property(x => x.RetryCount)
                    .IsRequired();

                builder.HasIndex(x => x.ProcessedOnUtc);

                builder.HasIndex(x => x.OccurredOnUtc);
            });

        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<decimal>()
                .HavePrecision(18, 2);
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
        {
            return await this.Database.BeginTransactionAsync(cancellationToken);
        }

        public DbSet<Product> Products { set; get; }
        public DbSet<OutboxMessage> OutboxMessages { set; get; }
    }
}
