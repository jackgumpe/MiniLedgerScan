using Microsoft.Extensions.Options;
using MiniLedgerScan.Api.Application;
using MiniLedgerScan.Api.Configuration;
using MiniLedgerScan.Api.ErrorHandling;
using MiniLedgerScan.Api.Infrastructure;
using MiniLedgerScan.Api.Providers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IOcrProvider, MockBaiduOcrProvider>();
builder.Services.AddScoped<ProofPacketService>();

builder.Services
    .AddOptions<MockOcrOptions>()
        .Bind(
            builder.Configuration.GetSection(
                    MockOcrOptions.SectionName))
        .ValidateOnStart();

builder.Services.AddSingleton<IValidateOptions<MockOcrOptions>,
         MockOcrOptionsValidator>();
builder.Services.AddSingleton<IProofPacketStore,InMemoryProofPacketStore>();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<OcrProviderUnavailableExceptionHandler>();
builder.Services.AddExceptionHandler<OperationCanceledExceptionHandler>();
    
var app = builder.Build();

app.UseExceptionHandler();

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
