using PaymentGateway.Api.AcquiringBankAdapter;
using PaymentGateway.Api.AcquiringBankClient;
using PaymentGateway.Api.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpClient<IAcquiringBankAdapter, AcquiringBankAdapter>(client =>
    client.BaseAddress = builder.Configuration.GetSection("AcquiringBank:BaseUrl").Get<Uri>());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();

builder.Services.AddSingleton<PaymentsRepository>();

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