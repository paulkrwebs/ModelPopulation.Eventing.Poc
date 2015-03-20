using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelPopulation.Eventing
{
    public class GenericParameterInfo
    {
        public Type[] Contraints { get; set; }

        public string Name { get; set; }

        public Type Type { get; set; }

        public bool IsParameter { get; set; }
    }
}
