using FluentValidation;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Providers;
using PaymentGateway.Domain;

namespace PaymentGateway.Api.Validators;

public class PostPaymentRequestValidator : AbstractValidator<PostPaymentRequest>
{
    public PostPaymentRequestValidator(IDateTimeProvider dateTimeProvider)
    {
        // Card
        RuleFor(request => request.CardNumber)
            .NotEmpty()
            .Length(14, 19)
            .Matches(@"^\d+$").WithMessage("Card number must only contain numeric characters.");

        RuleFor(request => request.ExpiryMonth)
            .InclusiveBetween(1, 12);

        RuleFor(request => new { request.ExpiryMonth, request.ExpiryYear})
            .Must(expiry =>
            {
                string formattedExpiryDate = $"{expiry.ExpiryMonth.ToString().PadLeft(2, '0')}/{expiry.ExpiryYear}";
                bool doesDateParse = DateTime.TryParseExact(formattedExpiryDate, "MM/yyyy", null,
                    System.Globalization.DateTimeStyles.None, out DateTime expiryDate);
                
                return doesDateParse && expiryDate > dateTimeProvider.UtcNow;
            }).WithMessage("The expiry date must be in the future.");
        
        RuleFor(request => request.Cvv)
            .NotEmpty()
            .Length(3, 4)
            .Matches(@"^\d+$").WithMessage("CVV must only contain numeric characters.");
        
        // Currency
        RuleFor(request => request.Currency)
            .NotEmpty().WithMessage("Currency code is required.")
            .Must(code => !string.IsNullOrWhiteSpace(code) && Currency.ValidCurrencies.Contains(code))
            .WithMessage($"Currency must be one of the supported ISO codes: {string.Join(", ", Currency.ValidCurrencies)}");
        
        // Money
        RuleFor(request => request.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than 0.");

        RuleFor(request => request.Currency)
            .NotNull().WithMessage("Currency is required.");
    }
}