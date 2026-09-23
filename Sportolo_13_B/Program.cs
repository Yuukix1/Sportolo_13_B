using SportoloEredmenyApi.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Egyetlen, konfigurációból dolgozó factory, amely MySqlConnection példányokat ad.
builder.Services.AddSingleton<DbConnectionFactory>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // https://localhost:xxxx/scalar/v1
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
