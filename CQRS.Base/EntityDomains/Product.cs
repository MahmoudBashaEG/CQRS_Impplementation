using CQRS.Domain.Enums;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CQRS.Domain.EntityDomains
{
    public class Product : Base<int>
    {
        public string Name { get; set; }
        public int Stock { get; set; }
        public decimal UnitPrice { get; set; }
        public ProductStatuses ProductStatus { get; set; }

        public Span<int> Make()
        {
/*            Span<int> span = stackalloc int[5];
            return span;*/

            Span<int> span = new int[5];
            return span;

        }
    }
}


// Business process managment
// state mangament design pattern
// Open Telemintry
// MassTransit