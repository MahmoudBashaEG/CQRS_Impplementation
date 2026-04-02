using System;
using System.Collections.Generic;
using System.Text;

namespace CQRS.Application.DTOs.Product
{
    public class ProductDto 
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public bool InStock { get; set; } 
    }
}
