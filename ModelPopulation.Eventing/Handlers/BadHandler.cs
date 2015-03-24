using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelPopulation.Eventing.Models;
using ModelPopulation.Eventing.ViewModels;

namespace ModelPopulation.Eventing.Handlers
{
    public class BadHandler<TFrom, TTo> : IBadHandler<TFrom, TTo>
        where TFrom : Employee
        where TTo : EmployeeViewModel
    {
    }
}
