using CQRS.Application.Interfaces.Infrastructure;
using CQRS.Domain.EntityDomains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace CQRS.Infrastructure.Persistence.ReadDb
{
    // Add-Migration InitialReadDb -Context ReadDbContext -Project CQRS.Infrastructure -StartupProject CQRS.API -OutputDir Persistence\ReadDb\Migrations
    // Update-Database -Context ReadDbContext -Project CQRS.Infrastructure -StartupProject CQRS.API
    public class ReadDbContext : DbContext, IReadDbContext
    {
        public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(builder =>
            {
                builder.HasKey(x => x.Id);

                builder.Property(x => x.Id)
                    .ValueGeneratedNever();
            });

            modelBuilder.Entity<InboxMessage>(builder =>
            {
                builder.HasKey(x => new { x.MessageId, x.HandlerName });

                builder.Property(x => x.HandlerName)
                    .HasMaxLength(500)
                    .IsRequired();

                builder.Property(x => x.ProcessedOnUtc)
                    .IsRequired();
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
        public DbSet<InboxMessage> InboxMessages { set; get; }
    }
}
