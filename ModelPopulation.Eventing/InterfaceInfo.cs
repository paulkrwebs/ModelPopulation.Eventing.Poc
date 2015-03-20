using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelPopulation.Eventing
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Exrtract common properties from handler and interfaces to common base class</remarks>
    public class InterfaceInfo
    {
        public Type Type { get; set; }

        public GenericParameterInfo[] GenericParameterInfo { get; set; }

        public bool IsConstructed { get; set; }

        public bool IsGenericType { get; set; }
    }
}
