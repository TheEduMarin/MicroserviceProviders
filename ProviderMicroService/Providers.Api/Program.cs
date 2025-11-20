
using Providers.Domain;
using Providers.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<IProviderRepository>(sp =>
    new ProviderRepository(builder.Configuration.GetConnectionString("Default")!)
);

var app = builder.Build();
app.MapControllers();
app.Run();
