using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelPopulation.Eventing.Handlers
{
    interface IBadHandler : IHandler
    {
    }

    interface IBadHandler<TFrom, TTo> : IBadHandler
    {
    }
}
