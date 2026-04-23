using CQRS.Domain.EntityDomains;
using CQRS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CQRS.Application.Events.BrokerEvents.Product
{
    public class ProductCreatedEvent : Base<int>
    {
        public string Name { get; set; }
        public int Stock { get; set; }
        public decimal UnitPrice { get; set; }
        public ProductStatuses ProductStatus { get; set; }
    }
}
