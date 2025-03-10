using System.Reflection;
using System.Text.Json.Serialization;

using FluentValidation;
using FluentValidation.AspNetCore;

using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

using PaymentGateway.Adapters;
using PaymentGateway.Api;
using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Validators;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddAdapters(builder.Configuration);
builder.Services.AddPaymentGatewayServices();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<PostPaymentRequestValidator>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "My API - V1",
            Version = "v1"
        }
    );

    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(PaymentsController).Assembly.GetName().Name}.xml"));
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    // Disable typical problem details e.g type, title, status. Stops the default showing in swagger
    // TODO review this.
    options.SuppressMapClientErrors = true;
});

builder.Services.AddLogging();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();