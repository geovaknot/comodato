using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace _3M.Comodato.Front.Models.Validations
{
    public class RequiredIfAnyAttribute : ValidationAttribute
    {
        private readonly string[] _otherPropertyNames;

        public RequiredIfAnyAttribute(params string[] otherPropertyNames)
        {
            _otherPropertyNames = otherPropertyNames;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var properties = _otherPropertyNames.Select(validationContext.ObjectType.GetProperty);
            var values = properties.Select(p => p.GetValue(validationContext.ObjectInstance, null) as string);

            // Check if any of the other properties are filled
            if (values.Any(v => !string.IsNullOrEmpty(v)))
            {
                // Validate the current property
                if (value is string currentValue && !string.IsNullOrEmpty(currentValue))
                {
                    return ValidationResult.Success;
                }

                // If current property is empty, return validation error
                return new ValidationResult($"Conteúdo obrigatório");
            }

            return ValidationResult.Success;
        }
    }
}