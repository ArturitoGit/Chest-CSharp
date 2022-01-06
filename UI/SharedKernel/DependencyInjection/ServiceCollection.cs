using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using UI.Infrastructure;
using static UI.DependencyInjection.Service;

namespace UI.DependencyInjection
{
   /* 
    * This class is useful to send a request and get a result without caring about the handler.
    * The result must implement Service.IResult, which is an empty interface
    * The request must implement Service.IRequest<TResult>, which is an empty interface
    *
    * Usage is : 
    *   TResult result = ServiceCollection.Handle(request)
    */
    public class ServiceCollection
    {

        private static ServiceCollection _instance = null! ;

        public static ServiceCollection GetInstance ()
        {
            if (_instance is null)
            {
                _instance = new ServiceCollection() ;
            }
            return _instance ;
        }

        private Dictionary<Type,object> _scopes = new() ;

        public void RegisterScope<TInterface,TClass> ()
        where TClass : TInterface
        {
            if (_scopes.ContainsKey(typeof(TInterface)))  throw new ScopeAlreadyExistingException () ;

            //var instance = Activator.CreateInstance(typeof(TClass)) ;
            var instance = Invoker.Invoke(typeof(TClass)) ;
            if (instance is null) throw new ConstructorNotFoundException () ;

            _scopes.Add(typeof(TInterface), instance!) ;
        }
        public T GetScope<T> () 
        {
            try 
                { return (T) _scopes[typeof(T)] ; }
            catch (KeyNotFoundException)
                { throw new ScopeNotRegisteredException () ; }
        } 

        public object GetScope (Type t)
        {
            try 
                { return _scopes[t] ; }
            catch (KeyNotFoundException)
                { throw new ScopeNotRegisteredException () ; }
        }

        public static ICollection GetImplementations (Type t)
        {
            // Get all the types that inherit from t
            var implementation_types = typeof(ServiceCollection).Assembly.GetTypes()
                .Where(type => type.GetInterfaces().Contains(t)) ;

            // List of instances
            var implementations = (IList) Activator.CreateInstance(typeof(List<>).MakeGenericType(t))! ;
            
            foreach (var implementation_type in implementation_types)
            {
                try
                {
                    var instance = Activator.CreateInstance(implementation_type)! ;
                    implementations.Add(instance) ;
                }
                catch (Exception) {}
            }

            return implementations ;
        }

        public static Task<TResult> Handle<TResult> (IRequest<TResult> request)
        {
            // Look over all the classes to find a valid handler for the request
            var handler = typeof(ServiceCollection).Assembly.GetTypes()
                // The Handler should have an interface such as
                .Where(type => type.GetInterfaces()
                    // The interface of the handler should inherit from IRequestHandler
                    .Where(i => i.GetInterface(typeof(IRequestHandler).Name) != null)
                    .Select(i => (Interface : i, GenericArguments : i.GetGenericArguments()))
                    // The interface of the handler should have 2 generic arguments
                    .Where(i => i.GenericArguments.Count() == 2)
                    // The first generic argument of the handler's interface should be of the request type
                    .Where(i => i.GenericArguments[0] == request.GetType() )
                    // The second generic argument of the handler's interface should be of the result type
                    .Where(i => i.GenericArguments[1] == typeof(TResult))
                    .Any())
                .FirstOrDefault() ;

            // If a good handler could not be found
            if (handler is null) throw new ArgumentException(nameof(request)) ;

            // Look for the method implementation
            var interface_method = typeof(IRequestHandler<IRequest<IResult>,IResult>).GetMethods().First() ;
            var handle_method = handler.GetMethods()
                .Where(m => m.Name == interface_method.Name)
                .Where(m => m.ReturnType == typeof(Task<TResult>))
                .Where(m => m.GetParameters().Count() == 1)
                .Where(m => m.GetParameters()[0].ParameterType == request.GetType())
                .FirstOrDefault() ;

            // If the handler did not contain a valid Handle Method
            if (handle_method is null) throw new ArgumentException (nameof(request)) ;

            // Invoke the found method
            // var handler_instance = Activator.CreateInstance(handler) ;
            var handler_instance = Invoker.Invoke (handler) ;
            var result = handle_method.Invoke(handler_instance,new object[] {request}) ;

            // Convert the result to the right type
            return (Task<TResult>) result! ;
        }
        public static IValidator<T>[] GetValidators<T> ()
        {
            return typeof(ServiceCollection).Assembly.GetTypes()
                .Where(t => t.GetInterfaces().ToList().Contains(typeof(IValidator<T>)))
                .Select(t => Activator.CreateInstance(t))
                .Where(o => o is not null)
                .Select(o => (IValidator<T>) o!)
                .ToArray() ;
        }
    }

    [Serializable]
    internal class ConstructorNotFoundException : Exception
    {
        public ConstructorNotFoundException()
        {
        }

        public ConstructorNotFoundException(string? message) : base(message)
        {
        }

        public ConstructorNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected ConstructorNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    internal class ScopeNotRegisteredException : Exception
    {
        public ScopeNotRegisteredException()
        {
        }

        public ScopeNotRegisteredException(string message) : base(message)
        {
        }

        public ScopeNotRegisteredException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ScopeNotRegisteredException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    internal class ScopeAlreadyExistingException : Exception
    {
        public ScopeAlreadyExistingException()
        {
        }

        public ScopeAlreadyExistingException(string message) : base(message)
        {
        }

        public ScopeAlreadyExistingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ScopeAlreadyExistingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public class Service
    {
        public interface IRequest<TResult> {}
        public interface IResult {}

        public interface IRequestHandler {}
        public interface IRequestHandler<TRequest,TResult> : IRequestHandler 
            where TResult : IResult 
            where TRequest : IRequest<TResult> {
            Task<TResult> Handle(TRequest request) ;
        } 
    }
}