using FluentValidation;
using PaymentGateway.Api.Providers;
using PaymentGateway.Domain;

namespace PaymentGateway.Api.Validators;

public class CardDetailsValidator : AbstractValidator<CardDetails>
{
    public CardDetailsValidator(IDateTimeProvider dateTimeProvider)
    {
        RuleFor(card => card.FullCardNumber)
            .NotEmpty()
            .Length(14, 19)
            .Matches(@"^\d+$").WithMessage("Card number must only contain numeric characters.");

        RuleFor(card => card.ExpiryMonth)
            .InclusiveBetween(1, 12);

        RuleFor(card => new { card.ExpiryMonth, card.ExpiryYear, card.FormattedExpiryDate })
            .Must(expiry =>
            {
                bool doesDateParse = DateTime.TryParseExact($"{expiry.FormattedExpiryDate}", "MM/yyyy", null,
                    System.Globalization.DateTimeStyles.None, out DateTime expiryDate);
                
                return doesDateParse && expiryDate > dateTimeProvider.UtcNow;
            }).WithMessage("The expiry date must be in the future.");
        RuleFor(card => card.Cvv)
            .NotEmpty()
            .Length(3, 4)
            .Matches(@"^\d+$").WithMessage("CVV must only contain numeric characters.");
    }
}