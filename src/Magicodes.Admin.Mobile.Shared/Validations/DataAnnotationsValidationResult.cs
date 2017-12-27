using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Collections.Extensions;

namespace Magicodes.Admin.Validations
{
    public class DataAnnotationsValidationResult
    {
        public List<ValidationError> ValidationErrors { get; }

        public bool IsValid => !ValidationErrors.Any();

        public DataAnnotationsValidationResult()
        {
            ValidationErrors = new List<ValidationError>();
        }

        public DataAnnotationsValidationResult Add(string errorMessage, string memberName)
        {
            ValidationErrors.Add(new ValidationError(memberName, errorMessage));
            return this;
        }

        public string ConsolidatedMessage
        {
            get
            {
                if (ValidationErrors == null || !ValidationErrors.Any())
                {
                    return null;
                }

                return ValidationErrors
                    .Select(x => "* " + x.ErrorMessage)
                    .JoinAsString(Environment.NewLine);
            }
        }

        public DataAnnotationsValidationResult AddRange(IEnumerable<ValidationError> validationErrors)
        {
            ValidationErrors.AddRange(validationErrors);
            return this;
        }

    }
}