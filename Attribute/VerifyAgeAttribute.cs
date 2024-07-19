using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace LokerMVC.Attribute;

public class VerifyAgeAttribute(int minAge, int maxAge) : ValidationAttribute , IClientModelValidator
{
    public int MinAge { get; private set; } = minAge;
    public int MaxAge { get; private set; } = maxAge;

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is DateTime birthDate)
        {
            int age = CalculateAge(birthDate);
            if (age < MinAge || age > MaxAge)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
        }
        return ValidationResult.Success;
    }

    private int CalculateAge(DateTime birthDate)
    {
        var now = DateTime.Today;
        var age = now.Year - birthDate.Year;
        if (now < birthDate.AddYears(age)) age--;
        return age;
    }

    public void AddValidation(ClientModelValidationContext context)
    {
        context.Attributes.Add("data-val-custom-verifyage", FormatErrorMessage(context.ModelMetadata.GetDisplayName()));
        context.Attributes.Add(
            "data-val-custom-verifyage-validationcallback",
            $@"function(options) {{
                var now = new Date();
                var birthDate = new Date(options.value);
                var age = now.getFullYear() - birthDate.getFullYear();
                if (now < birthDate.setFullYear(birthDate.getFullYear() + {MinAge}) || age > {MaxAge}) {{
                    return false;
                }}
                return true;
            }}");
    }

    public override string FormatErrorMessage(string name)
    {
        return string.Format(ErrorMessageString, MinAge, MaxAge);
    }
}
