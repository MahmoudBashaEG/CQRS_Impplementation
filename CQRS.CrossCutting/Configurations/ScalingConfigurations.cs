using System;
using System.Collections.Generic;
using System.Text;

namespace CQRS.CrossCutting.Configurations
{
    public class ScalingConfigurations
    {
        public bool IsReadingAndWritingTheSameDatabase { get; set; }
    }
}