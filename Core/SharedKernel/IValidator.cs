using System.Collections.Generic;
using System.Linq;

namespace Chest.Core.Infrastructure
{
    public interface IValidator <T>
    {
        (bool IsValid, string Error) validate (T target) ;
    }

    public class Validator {
        public static (bool IsValid, string[] Errors) Validate<T> (ICollection<IValidator<T>> validators, T target)
        {
            var errors = validators
                .Select(v => v.validate(target))
                .Where(v => !v.IsValid)
                .Select(v => v.Error) 
                .ToArray() ;
            
            return (errors.Length == 0, errors) ;
        }
    }
}