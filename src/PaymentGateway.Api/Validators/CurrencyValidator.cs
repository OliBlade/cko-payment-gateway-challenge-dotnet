using FluentValidation;

using PaymentGateway.Domain;

namespace PaymentGateway.Api.Validators;

public class CurrencyValidator : AbstractValidator<Currency>
{
    public CurrencyValidator()
    {
        RuleFor(currency => currency.Code)
            .NotEmpty().WithMessage("Currency code is required.")
            .Must(code => !string.IsNullOrWhiteSpace(code) && Currency.ValidCurrencies.Contains(code))
            .WithMessage($"Currency must be one of the supported ISO codes: {string.Join(", ", Currency.ValidCurrencies)}");
    }
}