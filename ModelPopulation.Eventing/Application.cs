using Microsoft.Practices.Unity;
using ModelPopulation.Eventing.Handlers;
using ModelPopulation.Eventing.Models;
using ModelPopulation.Eventing.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelPopulation.Eventing
{
    public class Application : IDisposable
    {
        // needs to be a list of Func<object> to have multiple handlers run
        private Dictionary<Type, Dictionary<Type, Func<object>>> _handlers;
        private IUnityContainer _container;

        public void Init()
        {
            _handlers = new Dictionary<Type, Dictionary<Type, Func<object>>>();
            _container = new UnityContainer();

            var handlerType = typeof(IHandler);
            Type[] types = FindTypesImplementing(handlerType);

            foreach (Type type in types)
            {
                Type[] interfaces = ListHandlerInterfaces(type);
                foreach (Type @interface in interfaces)
                {   
                    Type[] interfaceGenericArguments = @interface.GetGenericArguments();

                    // if this has no generic arguments then skip!
                    if (interfaceGenericArguments.Count() == 0) continue;

                    Type[] fromTypes = FindTypesImplementing(interfaceGenericArguments[0]);
                    Type[] toTypes = FindTypesImplementing(interfaceGenericArguments[1]);
                    BuildHandlersForTypeMap(type, fromTypes, toTypes, interfaces);
                }

                Type[] classGenericArguemnts = type.GetGenericArguments();

                // if this has no generic arguments then skip!
                if (classGenericArguemnts.Count() < 2) continue;

                Type[] classGenericArguemntFrom = FindTypesImplementing(classGenericArguemnts[0].GetGenericParameterConstraints()[0]);
                Type[] classGenericArguemntTo = FindTypesImplementing(classGenericArguemnts[1].GetGenericParameterConstraints()[0]);

                BuildHandlersForTypeMap(type, classGenericArguemntFrom, classGenericArguemntTo, interfaces);
            }
        }

        private void BuildHandlersForTypeMap(Type type, Type[] fromTypes, Type[] toTypes, Type[] interfaces)
        {
            foreach (Type @from in fromTypes)
            {
                // Dictionary<Type, Func<object>> fromDict = new Dictionary<Type, Func<object>>();
                foreach (Type to in toTypes)
                {
                    //fromDict.Add(to, () =>
                    //{
                    //    // this should be all done in a DI container. 
                    //    // When we have a DI container we won't need these MAD dictionaires ;)
                    //    Type specific = type.MakeGenericType(@from, to);
                    //    return specific.GetConstructor(Type.EmptyTypes).Invoke(null);
                    //});

                    // for each interface of type handler then map add a registration
                    Type typeToRegister = type;
                    if(type.IsGenericType)
                        typeToRegister = type.MakeGenericType(@from, to);

                    // have to get interfaces of this newly constructed generic type (if it was a genric type ;))
                    foreach (Type @interface in ListHandlerInterfaces(typeToRegister))
                    {
                        _container.RegisterType(@interface, typeToRegister, Guid.NewGuid().ToString());
                        System.Diagnostics.Debug.WriteLine(@interface.FullName + " to " + typeToRegister.FullName);
                    }
                }
                //_handlers.Add(@from, fromDict);
            }
        }


        public string RunMapper(string employeeName)
        { 
            Developer dev = new Developer() { Name = employeeName };
            DeveloperViewModel devViewModel = new DeveloperViewModel();

            // IOneToOneDataPopulation<Developer, DeveloperViewModel> mapper = _handlers[typeof(Developer)][typeof(DeveloperViewModel)]() as IOneToOneDataPopulation<Developer, DeveloperViewModel>;
            List<IOneToOneDataPopulation<Developer, DeveloperViewModel>> mappers = _container.ResolveAll<IOneToOneDataPopulation<Developer, DeveloperViewModel>>().ToList();

            foreach(IOneToOneDataPopulation<Developer, DeveloperViewModel> mapper in mappers)
                mapper.Map(dev, devViewModel);

            TechnicalArchitectViewModel v = new TechnicalArchitectViewModel();
            _container.ResolveAll<IOneToOneDataPopulation<TechnicalArchitect, TechnicalArchitectViewModel>>().ToList()[0].Map(new TechnicalArchitect() { CanUml = true }, v);

            return dev.Name;
        }

        private static Type[] ListHandlerInterfaces(Type type)
        {
            return type.GetInterfaces().Where(i => i.IsAssignableFrom(type) && i != typeof(IHandler) && i.IsInterface).ToArray();
        }

        private static Type[] FindTypesImplementing(Type type)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                        .Where(a => a.FullName.StartsWith("ModelPopulation"))
                        .SelectMany(s => s.GetTypes())
                        .Where(p => type.IsAssignableFrom(p) && p.IsClass).ToArray();
        }

        public void Dispose()
        {
            _container.Dispose();
        }
    }
}
