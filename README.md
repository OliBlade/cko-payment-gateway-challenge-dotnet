# Instructions for candidates

This is the .NET version of the Payment Gateway challenge. If you haven't already read this [README.md](https://github.com/cko-recruitment/) on the details of this exercise, please do so now. 

## Template structure
```
src/
    PaymentGateway.Api - a skeleton ASP.NET Core Web API
test/
    PaymentGateway.Api.Tests - an empty xUnit test project
imposters/ - contains the bank simulator configuration. Don't change this

.editorconfig - don't change this. It ensures a consistent set of rules for submissions when reformatting code
docker-compose.yml - configures the bank simulator
PaymentGateway.sln
```

Feel free to change the structure of the solution, use a different test library etc.

-------------------

## Steps
1. Solidify current implementation around retrieving payment
- Satisfy existing tests - Add missing requirement to return not found if payment does not exist.
- Refactor tests to share setup code. Rename to comply with "UnitOfWork_StateUnderTest_ExpectedBehavior". Install FluentAssertions (my preference).
- Add test to confirm "Status must be one of the following values Authorized, Declined".
- Implement "Status must be one of the following values Authorized, Declined".
- Switch response model from "PostPaymentResponse" to "GetPaymentResponse" which seems to be more logical based on the project structure.
- Update GetPaymentResponse access modifiers to make this model less susceptible to misuse.