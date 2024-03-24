using CollectionsApp.Enums;
using CollectionsApp.Models;
using CollectionsApp.ViewModels;
using System;
using System.ComponentModel.DataAnnotations;

public class ValidCustomFieldValueAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var model = (CustomFieldVM)validationContext.ObjectInstance;

        switch (model.customFieldType)
        {
            case CustomFieldTypes.Text:
            case CustomFieldTypes.MultilineText:
                // No specific validation for text fields
                return ValidationResult.Success;

            case CustomFieldTypes.Numeric:
                // Check if the value is numeric
                if (!IsNumeric(value))
                    return new ValidationResult("Please enter a numeric value.");
                break;

            case CustomFieldTypes.Logical:
                // Check if the value is either true or false
                if (!IsBoolean(value))
                    return new ValidationResult("Please enter 'true' or 'false'.");
                break;

            case CustomFieldTypes.DateTime:
                // Check if the value is a valid DateTime
                if (!IsDateTime(value))
                    return new ValidationResult("Please enter a valid date and time.");
                break;

            default:
                return new ValidationResult("Invalid field type.");
        }

        return ValidationResult.Success;
    }

    private bool IsNumeric(object value)
    {
        double result;
        return Double.TryParse(Convert.ToString(value), out result);
    }

    private bool IsBoolean(object value)
    {
        bool result;
        return Boolean.TryParse(Convert.ToString(value), out result);
    }

    private bool IsDateTime(object value)
    {
        DateTime result;
        return DateTime.TryParse(Convert.ToString(value), out result);
    }
}
