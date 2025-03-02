using PaymentGateway.Api.Models.Enums;

namespace PaymentGateway.Api.Models;

internal static class MappingExtensions
{
    internal static PaymentStatus ToModelPaymentStatus(this Domain.Enums.PaymentStatus status) => status switch
    {
        Domain.Enums.PaymentStatus.Authorized => PaymentStatus.Authorized,
        Domain.Enums.PaymentStatus.Declined => PaymentStatus.Declined,
        Domain.Enums.PaymentStatus.Rejected => PaymentStatus.Rejected,
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Status enum is not recognised in models")
    };
}