using CQRS.Application.Interfaces.Infrastructure;
using CQRS.Domain.EntityDomains;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace CQRS.Infrastructure.ApplicationReadDb
{
    public class ApplicationReadDbContext : DbContext, IReadDbContext
    {
        public ApplicationReadDbContext(DbContextOptions<ApplicationReadDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<decimal>()
                .HavePrecision(18, 2);
        }

        public DbSet<Product> Products { set; get; }
    }
}
