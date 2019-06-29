
using System.ComponentModel.DataAnnotations;

namespace Lxdn.Core._MSTests.Domain
{
    public class PersonValidator
    {
        public static ValidationResult Validate(Person person, ValidationContext context)
        {
            var svc = context.GetService(typeof(Person));
            //return ValidationResult.Success;
            return new ValidationResult("Something is wrong");
        }
    }
}