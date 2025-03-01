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

## Assumptions
- Based on the test description of the acquiring bank "It also performs some validation of the card information and then sends the payment details to the appropriate 3rd party organization for processing" it looks like
the acquiring bank reaches out to other third parties for us. Therefore I have assumed we don't need multiple implementations of acquiring banks, ideally I would clarify this as part of refinement/ discovery.
- The authorization_code returned from the acquiring bank is a GUID. 

## Steps Taken
1. Solidify current implementation of retrieving payment:
- Satisfy existing tests - Add missing requirement to return not found if payment does not exist.
- Refactor tests to share setup code. Rename to comply with "UnitOfWork_StateUnderTest_ExpectedBehavior". Install FluentAssertions (my preference).

- Tweak a couple of editorconfig settings to show types where ambiguous. IMO this is easier to read especially with nullables.
- Correct namespace of PaymentStatus.

- Add test to confirm "Status must be one of the following values Authorized, Declined".
- Implement "Status must be one of the following values Authorized, Declined".

- Switch response model from "PostPaymentResponse" to "GetPaymentResponse" which seems to be more logical based on the project structure.
- Update GetPaymentResponse access modifiers to make this model less susceptible to misuse.

- Added built in logging via `.AddLogging()`. There are other options available such as serilog etc but this can be considered later.

2. Using the adapter pattern, add in a http client to communicate with the acquiring bank.
- Add tests for the acquiring bank adapter.
- Add contracts for the acquiring bank adapter.

