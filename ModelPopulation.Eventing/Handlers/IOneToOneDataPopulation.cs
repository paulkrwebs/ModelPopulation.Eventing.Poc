using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelPopulation.Eventing.Handlers
{
    public interface IOneToOneDataPopulation<TFrom, TTo> : IHandler
    {
        void Map(TFrom @from, TTo to);
    }
}
