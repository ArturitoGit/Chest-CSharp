using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UI.DependencyInjection
{

    /**
     *
     * This class is used in ServiceCollection to instanciate RequestHandlers
     * by binding the parameters from the Scope
     *
     */
    public class Invoker
    {
        public static object Invoke (Type t)
        {
            // Look for a constructor in T class
            var constructors = t.GetConstructors() ;

            // If no constructor is found
            if (constructors.Length <= 0) throw new System.Exception ($"No constructor was found for class {t.Name}") ;

            // Try to build each one of the found constructors
            foreach (var constructor in constructors)
            {
                //try
                //{
                    return InvokeConstructor (constructor) ;
                //}
                //catch ( Exception ) {}
            }

            // If no valid constructor is found
            throw new Exception ($"No valid constructor was found for class {t.Name}") ;   
        }
        public static object InvokeConstructor (ConstructorInfo constructor)
        {
            // Try to build all the parameters
            List<object?> parameters = new () ;
            foreach (var t in constructor.GetParameters())
            {
                // Collection of scopes
                if (t.ParameterType.IsGenericType && t.ParameterType.GetGenericTypeDefinition() == typeof(ICollection<object>).GetGenericTypeDefinition())
                {
                    // Get the sub type of the collection
                    Type sub_type = t.ParameterType.GetGenericArguments()[0] ;
                    var instances = ServiceCollection.GetImplementations(sub_type) ;
                    parameters.Add(instances) ;
                }
                else
                {
                    // Scope member
                    var instance = ServiceCollection.GetInstance().GetScope(t.ParameterType) ;
                    parameters.Add(instance) ;
                }
            }

            return constructor.Invoke(parameters.ToArray()) ;
        }
    }
}