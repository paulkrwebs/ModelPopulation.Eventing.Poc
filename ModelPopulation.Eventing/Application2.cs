using Microsoft.Practices.Unity;
using ModelPopulation.Eventing.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelPopulation.Eventing
{
    public class Application2 : IDisposable
    {
        private List<HandlerInfo> _handlers;
        private IUnityContainer _container;
        private Type _handleType;

        public void Init()
        {
            _handlers = new List<HandlerInfo>();
            _container = new UnityContainer();
            _handleType = typeof(IHandler);

            Type[] types = FindTypesImplementing(_handleType);

            foreach (Type type in types)
            {
                _handlers.Add(CreateHandleInfoRegistration(type));
            }
        }

        public void Test<TEvent>()
            where TEvent : IHandler
        {
            // this isn't thread safe so need to change
            if (_container.IsRegistered<TEvent>()) return;

            List<HandlerInfo> handlersToRegister = new List<HandlerInfo>();

            FindHandlers<TEvent>(handlersToRegister);

            // construct generic types
            // register all newly constructed types and already constructed types with interface TEvent
        }

        public void FindHandlers<TEvent>(List<HandlerInfo> handlersToRegister)
            where TEvent : IHandler
        {
            HandlerInfo eventInfo = CreateHandleInfoRegistration(typeof(TEvent));

            foreach (HandlerInfo handler in _handlers)
            {
                // exact match not working
                if (IsExactMatch(eventInfo, handler))
                {
                    AddToRegisteredHandlers(handlersToRegister, handler);
                    continue;
                }

                if(IsGenericsMatch(handlersToRegister, eventInfo, handler))
                    AddToRegisteredHandlers(handlersToRegister, handler);
            }
        }

        private static bool IsGenericsMatch(List<HandlerInfo> handlersToRegister, HandlerInfo eventInfo, HandlerInfo handler)
        {
            if (!handler.IsGenericType)
                return false;

            // Match types that are generic and not constructed
            if (IsGenericClassMatched(eventInfo, handler))
                return true;

            // Match types that are not a genric type
            // Therefore the interface must have been constructed so we want to check the generic parameters and not the parameter constrains
            if (IsConstructedGenericInterfaceOnClassMatched(eventInfo, handler))
                return true;

            return false;
        }

        private static bool IsExactMatch(HandlerInfo eventInfo, HandlerInfo handler)
        {
            return handler.Type == eventInfo.Type || handler.Interfaces.Any(i => i.Type == eventInfo.Type);
        }

        private static bool NumberOfGenericParametersMatch(HandlerInfo eventInfo, HandlerInfo handler)
        {
            return handler.GenericParameterInfo.Count() == eventInfo.GenericParameterInfo.Count();
        }

        private static void AddToRegisteredHandlers(List<HandlerInfo> handlersToRegister, HandlerInfo handler)
        {
            if (!handlersToRegister.Contains(handler))
                handlersToRegister.Add(handler);
        }

        private static bool IsGenericClassMatched(HandlerInfo eventInfo, HandlerInfo handler)
        {
            if (!IsValidGenericClassToCompare(eventInfo, handler))
                return false;

            for (int counter = 0; counter < handler.GenericParameterInfo.Count(); counter++)
            {
                // Are any of the constrains on the generic parameters assignable to the event type e.g the "where constrainsts"
                if (handler.GenericParameterInfo[counter].Contraints
                    .Any(hc => hc.IsAssignableFrom(eventInfo.GenericParameterInfo[counter].Type))
                    && IsInterfaceMatched(eventInfo, handler))
                    continue;

                return false;
            }

            return true;
        }

        private static bool IsInterfaceMatched(HandlerInfo eventInfo, HandlerInfo handler)
        {
            if (!eventInfo.Type.IsInterface)
                return true;

            if (!handler.Interfaces.Any())
                return false;

            if (!handler.Interfaces.Any(i => i.IsGenericType && i.Type.GetGenericTypeDefinition() == eventInfo.Type.GetGenericTypeDefinition()))
                return false;

            // if the interface is constructed I need to do an exact match

            return true;
        }

        private static bool IsValidGenericClassToCompare(HandlerInfo eventInfo, HandlerInfo handler)
        {
            if (!handler.IsGenericType)
                return false;

            if (handler.IsConstructed)
                return false;

            if (!NumberOfGenericParametersMatch(eventInfo, handler))
                return false;

            return true;
        }

        private static bool IsConstructedGenericInterfaceOnClassMatched(HandlerInfo eventInfo, HandlerInfo handler)
        {
            foreach (InterfaceInfo interfaceInfo in handler.Interfaces)
            {
                if (!interfaceInfo.IsGenericType)
                    return false;

                for (int i = 0; i < interfaceInfo.GenericParameterInfo.Count(); i++)
                {
                    // Are the generic parameters an exact match for the interface parameters
                    // we don't want to use IsAssignableFrom because we only want to allow inheritance with Generic Parameter constraints
                    if (interfaceInfo.GenericParameterInfo[i].Type == eventInfo.GenericParameterInfo[i].Type) continue;

                    return false;
                }
            }
            return true;
        }

        private HandlerInfo CreateHandleInfoRegistration(Type type)
        {
            if(type.IsGenericType)
                return new HandlerInfo()
                {
                    IsGenericType = true,
                    IsConstructed = type.IsConstructedGenericType,
                    GenericParameterInfo = ListGenericParameterInfo(type),
                    Type = type,
                    Interfaces = ListHandlerInterfacesInfo(type)
                };

            return new HandlerInfo() { Type = type, Interfaces = ListHandlerInterfacesInfo(type) };
        }

        private InterfaceInfo[] ListHandlerInterfacesInfo(Type type)
        {
            return ListHandlerInterfaces(type)
                .Select(t => CreateInterfaceInfo(t))
                .ToArray();
        }

        private static InterfaceInfo CreateInterfaceInfo(Type t)
        {
            if(t.IsGenericType)
                return new InterfaceInfo() 
                { 
                    IsGenericType = true, 
                    GenericParameterInfo = ListGenericParameterInfo(t), 
                    Type = t, 
                    IsConstructed = t.IsConstructedGenericType 
                };

            return new InterfaceInfo() { Type = t };
        }

        private static GenericParameterInfo[] ListGenericParameterInfo(Type type)
        {
            return type.GetGenericArguments()
                .Where(t => t.IsClass)
                .Select(t => new GenericParameterInfo() { Type = t, Name = t.Name, Contraints = t.IsGenericParameter ? t.GetGenericParameterConstraints() : new Type[0], IsParameter = t.IsGenericParameter })
                .ToArray();
        }

        private Type[] ListHandlerInterfaces(Type type)
        {

            return type.GetInterfaces()
                .Where(i => _handleType.IsAssignableFrom(type) && i != _handleType && i.IsInterface)
                .ToArray();
        }

        private Type[] FindTypesImplementing(Type type)
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
