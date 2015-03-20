using ModelPopulation.Eventing.Models;
using ModelPopulation.Eventing.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelPopulation.Eventing.Handlers
{
    public class EmployeeHandler<TFrom, TTo> : IOneToOneDataPopulation<TFrom, TTo>, IFoo
        where TFrom : Employee
        where TTo : EmployeeViewModel
    {
        public void Map(TFrom from, TTo to)
        {
            to.FullName = from.Name;
        }
    }
}
