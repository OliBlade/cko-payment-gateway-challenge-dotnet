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

[Requirements](https://github.com/cko-recruitment/#requirements)

## Assumptions
- Based on the test description of the acquiring bank "It also performs some validation of the card information and then sends the payment details to the appropriate 3rd party organization for processing" it looks like
the acquiring bank reaches out to other third parties for us. Therefore, I have assumed we don't need multiple implementations of acquiring banks, ideally I would clarify this as part of refinement/ discovery.

## Design Considerations~~~~
1. Domain Driven Design - I have opted to use a DDD approach to keep the project align with business objectives.
2. Clean Architecture - A clean architecture approach helps keep the project maintainable and testable.
3. Adapter Pattern - The adapter pattern is used to abstract the acquiring bank away from the main project. This allows us to swap out the acquiring bank if or when needed.
4. Repository Pattern - Following the repository pattern allows us to easily swap out the storage solution if needed. This follows the way the project was initially setup.
5. Testing strategy -  A testing pyramid style approach has been used to ensure the project is well tested. Unit tests are predominantly used with a few integration tests to ensure the project works as expected.
6. Logging - I've gone with a minimal logging approach using the built in logging in .NET Core. This can be easily swapped out for a more robust logging solution.

## Future Considerations/ Production Readiness
1. Implement authentication and authorization.
2. Swap out the in-memory storage for a more robust solution such as a database.
3. Encrypt all communications, IE use https etc.
4. Comply with legal requirements such as GDPR especially when storing card details.
5. Increase logging, add metrics and general observability.
6. Use Autofixture or a cleaner way to create test data. Cover edge cases.

## Further thoughts
- Should this API be aware of a merchant? At least an Id might be good for future reference.
- Should processing a payment be idempotent? Currently multiple requests would perform a new payment which may not be intentional.
- Versioning the Api - This would allow us to make updates to the API without breaking existing clients.
- Consider the implications of a payment processing but not being able to store the payment. IE storage error.
- Testing requires the bank simulator to be running, this should either be run in the build pipeline or could be swapped out for a mock or stub.
- Load testing - This would be a good idea to ensure the API can handle the expected load.
- Cancellation token - Should we be able to cancel a payment processing request? What are the implications of this?

## Ste[.editorconfig](.editorconfig)ps Taken
1. Solidify current implementation of retrieving payment:
   - Satisfy existing tests - Add missing requirement to return not found if payment does not exist.
   - Refactor tests to share setup code. Rename to comply with "UnitOfWork_StateUnderTest_ExpectedBehavior". Install FluentAssertions (my preference).
   - Correct namespace of PaymentStatus.
   - Add test to confirm "Status must be one of the following values Authorized, Declined".
   - Protect against returning rejected payments "Status must be one of the following values Authorized, Declined".
   - Switch response model from "PostPaymentResponse" to "GetPaymentResponse" which seems to be more logical based on the project structure.
   - Update GetPaymentResponse access modifiers to make this model less susceptible to misuse.
   - Added built in logging via `.AddLogging()`. There are other options available such as serilog etc but this can be considered later.

2. Using the adapter pattern, add in a http client to communicate with the acquiring bank.
   - Add tests for the acquiring bank adapter.
   - Add contracts for the acquiring bank adapter.

3. Build the Payment domain model. Gives us fine control over the data and access:
   - Update the model last four digits to be a string. This is in case the first digit is a 0. This assumes this api is not in use.
   - Move adapters into own project, new project for domain models.
   - Add tests for the domain models.
   - Update repository and controller to use the domain models.
   - Start work on the payment processor.

4. Add in the payment processor:
   - Tweak the result of the acquiring bank to be a bit more usable.
   - Add tests for the payment processor.
   - Give the repository an interface for testing purposes, and to later swap with a better storage solution.
   - Start work on the process payment endpoint.

5. Implement the process payment endpoint:
   - Get the integration tests working using the bank simulator.
   - Add in basic validation for the model.
   - Add in fluent validation to validate the payment request.
   - Updated Readme.
