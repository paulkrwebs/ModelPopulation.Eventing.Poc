using ModelPopulation.Eventing.Handlers;
using ModelPopulation.Eventing.Models;
using ModelPopulation.Eventing.ViewModels;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelPopulation.Eventing
{
    [TestFixture]
    public class Class1
    {
        [Test]
        public void Run()
        {
            //using (Application application = new Application())
            //{
            //    application.Init();

            //    Assert.That(application.RunMapper("paul"), Is.EqualTo("paul"));
            //}

            // new technique
            // 1) @Start up, find all types that implement IHander
            // 2) If generic type then find interface that implements IHander
            // 3) Get all generic parameter contrains from type or interface
            // 4) @Resolve, if current type isn't registered. 
            // a) If not a generic type find all types that implement
            // b) If is generic type find all parameter and for each pamaater see if it can be basted to the contraint of the generic parmaters pre-loaded at startup
            // c) Register all matches

            using (Application2 app2 = new Application2())
            {
                app2.Init();
                // expect 3 registrations
                List<HandlerInfo> handlersToRegister = new List<HandlerInfo>();
                app2.FindHandlers<IOneToOneDataPopulation<TechnicalArchitect, TechnicalArchitectViewModel>>(handlersToRegister);
                Assert.That(handlersToRegister.Count(), Is.EqualTo(3));

                // expect 3 regs
                handlersToRegister = new List<HandlerInfo>();
                app2.FindHandlers<IOneToOneDataPopulation<Developer, DeveloperViewModel>>(handlersToRegister);
                Assert.That(handlersToRegister.Count(), Is.EqualTo(2));

                // expect 1 regs
                handlersToRegister = new List<HandlerInfo>();
                app2.FindHandlers<IOneToOneDataPopulation<ProjectManager, ProjectManagerViewModel>>(handlersToRegister);
                Assert.That(handlersToRegister.Count(), Is.EqualTo(1));

                // expect 0 regs
                handlersToRegister = new List<HandlerInfo>();
                app2.FindHandlers<IOneToOneDataPopulation<object, object>>(handlersToRegister);
                Assert.That(handlersToRegister.Count(), Is.EqualTo(0));
            }
        }
    }
}
