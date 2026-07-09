using System;
using System.Collections.Generic;
using System.Text;

namespace CQRS.Infrastructure.Configurations
{
    public class ScalingConfigurations
    {
        public bool IsReadingAndWritingTheSameDatabase { get; set; }
    }
}