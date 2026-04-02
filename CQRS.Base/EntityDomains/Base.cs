using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CQRS.Domain.EntityDomains
{
    public class Base<T> where T : struct
    {
        [Key]
        public T Id { get; set; }
    }
}
