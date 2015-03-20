using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelPopulation.Eventing
{
    public class HandlerInfo
    {
        public GenericParameterInfo[] GenericParameterInfo { get; set; }

        public Type Type { get; set; }

        public InterfaceInfo[] Interfaces { get; set; }

        public bool IsConstructed { get; set; }

        public bool IsGenericType { get; set; }
    }
}
