using FluentValidation;

using PaymentGateway.Api.Providers;
using PaymentGateway.Domain;

namespace PaymentGateway.Api.Validators;

public class PaymentValidator : AbstractValidator<Payment>
{
    public PaymentValidator(IDateTimeProvider dateTimeProvider)
    {
        RuleFor(payment => payment.CardDetails)
            .NotNull().WithMessage("CardDetails is required.");
        
        RuleFor(payment => payment.CardDetails)
            .NotNull().SetValidator(new CardDetailsValidator(dateTimeProvider));
        
        RuleFor(payment => payment.Amount)
            .NotNull().WithMessage("Amount is required.");

        RuleFor(payment => payment.Amount)
            .NotNull().SetValidator(new MoneyValidator());
    }
}