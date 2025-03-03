using FluentValidation;

using PaymentGateway.Domain;

namespace PaymentGateway.Api.Validators;

public class MoneyValidator : AbstractValidator<Money>
{
    public MoneyValidator()
    {
        RuleFor(money => money.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than 0.");

        RuleFor(money => money.Currency)
            .NotNull().WithMessage("Currency is required.");

        RuleFor(money => money.Currency)
            .SetValidator(new CurrencyValidator());
    }
}