using FluentValidation.Results;

namespace ContactNumberWebAPI.Common;

public static class ValidationErrorMapper
{
    public static IReadOnlyList<string> ToMessages(ValidationResult validationResult)
    {
        return validationResult.Errors
            .Select(error => error.ErrorMessage)
            .Where(message => !string.IsNullOrWhiteSpace(message))
            .Distinct()
            .ToArray();
    }
}