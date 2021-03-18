using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TestSysplan.Core.Model
{
    public partial class Client : IValidatableObject
    {
        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            if(Age < 1 || Age > 200)
            {
                yield return new ValidationResult("Invalid Age value.", new[] { nameof(Age) });
            }
            else if(string.IsNullOrWhiteSpace(Name))
            {
                yield return new ValidationResult("Invalid Name!", new[] { nameof(Name) });
            }
            else if(Name.Length > 100)
            {
                yield return new ValidationResult("Name value is so large, input a value equals or less than 100 chars!", new[] { nameof(Name) });
            }
        }
    }
}
