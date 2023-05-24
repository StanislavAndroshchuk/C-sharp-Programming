using System.ComponentModel.DataAnnotations;

namespace API_Task.Dto;
public class DateGreaterThanAttribute : ValidationAttribute
{
    private readonly string _comparisonProperty;

    public DateGreaterThanAttribute(string comparisonProperty)
    {
        _comparisonProperty = comparisonProperty;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null) return ValidationResult.Success;
        var currentValue = (DateOnly)value;

        var comparisonValue = (DateOnly)validationContext.ObjectType
            .GetProperty(_comparisonProperty)!
            .GetValue(validationContext.ObjectInstance)!;

        if (currentValue > comparisonValue)
        {
            return ValidationResult.Success!;
        }
        
        return new ValidationResult(ErrorMessage = "ShippedDate must be greater than OrderDate");
    }
}