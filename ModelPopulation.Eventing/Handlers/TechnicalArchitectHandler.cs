using ModelPopulation.Eventing.Models;
using ModelPopulation.Eventing.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelPopulation.Eventing.Handlers
{
    public class TechnicalArchitectHandler : IOneToOneDataPopulation<TechnicalArchitect, TechnicalArchitectViewModel>
    {
        public void Map(TechnicalArchitect from, TechnicalArchitectViewModel to)
        {
            to.CanUml = from.CanUml;
        }
    }
}
