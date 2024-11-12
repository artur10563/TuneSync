using Domain.Errors;
using FluentValidation.Results;

namespace Application.Extensions
{
    public static class ValidationResultToErrorExtension
    {
        public static List<Error> AsErrors(this ValidationResult validationResult)
        {
            if (validationResult.IsValid || validationResult.Errors.Count == 0)
                return [];

            return validationResult.Errors
                .Select(x => new Error(x.ErrorMessage))
                .ToList();
        }
    }
}
