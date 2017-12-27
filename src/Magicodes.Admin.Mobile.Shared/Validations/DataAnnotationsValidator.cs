using System;
using Magicodes.Admin.Localization;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Magicodes.Admin.Validations
{
    public static class DataAnnotationsValidator
    {
        public static DataAnnotationsValidationResult Validate(object input)
        {
            var dataAnnotationsValidationResult = new DataAnnotationsValidationResult();

            var nestedValidationProperties = input.GetType().GetProperties()
                .Where(p => p.IsDefined(typeof(ValidationAttribute)))
                .OrderBy(p => p.Name);

            foreach (var property in nestedValidationProperties)
            {
                var propertyName = property.Name;
                var validators = property.GetCustomAttributes(typeof(ValidationAttribute)) as ValidationAttribute[];

                if (validators == null || validators.Length == 0) continue;

                foreach (var validator in validators)
                {
                    var propertyValue = property.GetValue(input, null);
                    var result = validator.GetValidationResult(propertyValue, new ValidationContext(input, null, null));
                    if (result == ValidationResult.Success)
                    {
                        continue;
                    }

                    dataAnnotationsValidationResult.Add(GetMessageByAttribute(validator, propertyName), propertyName);
                }
            }

            return dataAnnotationsValidationResult;
        }

        private static string GetMessageByAttribute(ValidationAttribute validationAttribute, string propertyName)
        {
            switch (validationAttribute)
            {
                case RequiredAttribute _:
                    return L.Localize("RequiredField", L.Localize(propertyName));
                case EmailAddressAttribute _:
                    return L.Localize("InvalidEmailAddress");
                case StringLengthAttribute _:
                    var stringLengthAttribute = (StringLengthAttribute)validationAttribute;
                    return L.Localize("LengthNotInRange", L.Localize(propertyName), stringLengthAttribute.MinimumLength, stringLengthAttribute.MaximumLength);
                case MaxLengthAttribute _:
                    var maxLengthAttribute = (MaxLengthAttribute)validationAttribute;
                    return L.Localize("MoreThanMaxStringLength", L.Localize(propertyName), maxLengthAttribute.Length);
                case MinLengthAttribute _:
                    var minLengthAttribute = (MinLengthAttribute)validationAttribute;
                    return L.Localize("LessThanMinStringLength", L.Localize(propertyName), minLengthAttribute.Length);
                case PhoneAttribute _:
                    return L.Localize("InvalidPhoneNumber", L.Localize(propertyName));
                case RegularExpressionAttribute _:
                    return L.Localize("InvalidRegularExpression", L.Localize(propertyName));
                case RangeAttribute _:
                    var rangeAttribute = (RangeAttribute)validationAttribute;
                    return L.Localize("LengthNotInRange", L.Localize(propertyName), rangeAttribute.Minimum, rangeAttribute.Maximum);

                //localize rest of other attributes when needed...
                /*
                case CompareAttribute compareAttribute:
                    break;
                case CreditCardAttribute creditCardAttribute:
                    break;
                case EnumDataTypeAttribute enumDataTypeAttribute:
                    break;
                case FileExtensionsAttribute fileExtensionsAttribute:
                    break;
                case UrlAttribute urlAttribute:
                    break;
                case DataTypeAttribute dataTypeAttribute:
                    break;
                */
                default:
                    throw new ArgumentException();
            }
        }
    }
}