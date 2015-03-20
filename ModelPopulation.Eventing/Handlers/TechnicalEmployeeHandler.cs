using ModelPopulation.Eventing.Models;
using ModelPopulation.Eventing.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelPopulation.Eventing.Handlers
{
    public class TechnicalEmployeeHandler<TFrom, TTo> : IOneToOneDataPopulation<TFrom, TTo>
        where TFrom : TechnicalEmployee
        where TTo : TechnicalEmployeeViewModel
    {
        public void Map(TFrom from, TTo to)
        {
            to.TechnicalLevel = from.TechnicalLevel;
        }
    }
}
