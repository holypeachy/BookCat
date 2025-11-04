using System.ComponentModel.DataAnnotations;

public class ConfirmTextAttribute : ValidationAttribute
{
    private readonly string _expectedText;

    public ConfirmTextAttribute(string expectedText)
    {
        _expectedText = expectedText;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null)
            return new ValidationResult(ErrorMessage ?? $"You must type {_expectedText}.");

        var input = value.ToString()?.Trim();

        return string.Equals(input, _expectedText, StringComparison.OrdinalIgnoreCase)
            ? ValidationResult.Success
            : new ValidationResult(ErrorMessage ?? $"You must type {_expectedText}.");
    }
}
