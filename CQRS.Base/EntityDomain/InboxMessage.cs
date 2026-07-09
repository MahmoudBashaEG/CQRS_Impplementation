using System;
using System.Collections.Generic;
using System.Text;

namespace CQRS.Domain.EntityDomains
{
    public sealed class InboxMessage
    {
        public Guid MessageId { get; set; }

        public string HandlerName { get; set; } = null!;

        public DateTime ProcessedOnUtc { get; set; }
    }
}
