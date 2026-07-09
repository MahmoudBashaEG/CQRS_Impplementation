using CQRS.Domain.EntityDomains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace CQRS.Application.Interfaces.Infrastructure
{
    public interface IReadDbContext
    {
        DbSet<Product> Products { get; }
        DbSet<InboxMessage> InboxMessages { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
    }
}
