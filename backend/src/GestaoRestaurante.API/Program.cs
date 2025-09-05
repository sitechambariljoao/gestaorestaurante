using GestaoRestaurante.API.Extensions;
using GestaoRestaurante.Shared.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    // Adicionar filtro global de validação
    options.Filters.Add<GestaoRestaurante.API.Filters.ValidationFilter>();
});

// Configurar logging customizado
builder.Services.AddCustomLogging();

// Configurar Infrastructure (DbContext e Repositories)
builder.Services.AddInfrastructure(builder.Configuration);

// Configurar Application (Services)
builder.Services.AddApplication();

// Configurar Action Filters
builder.Services.AddActionFilters();

// Configurar Identity e JWT
builder.Services.AddIdentityAndJwt(builder.Configuration);

// Configurar Swagger/OpenAPI
builder.Services.AddSwagger();

// Configurar CORS
builder.Services.AddCustomCors();

// Configurar Health Checks avançados (Fase 5)
builder.Services.AddAdvancedHealthChecks();

// Configurar JSON options
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = null; // Manter nomes originais
    options.SerializerOptions.WriteIndented = true;
});

builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null; // Manter nomes originais
    options.JsonSerializerOptions.WriteIndented = true;
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gestão Restaurante API v1.0.0");
        c.RoutePrefix = "swagger";
        c.DocumentTitle = "Gestão Restaurante - API Documentation";
        c.DefaultModelsExpandDepth(-1);
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        c.EnableDeepLinking();
        c.EnableFilter();
        c.ShowExtensions();
    });
}
else
{
    // Swagger também em produção para facilitar testes (remover se necessário)
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gestão Restaurante API v1.0.0");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

// Usar middlewares customizados (na ordem correta)
app.UseCustomMiddlewares();

// Usar CORS
app.UseCors(builder.Environment.IsDevelopment() ? "Development" : "Production");

// Usar Authentication e Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Configurar Health Checks endpoints (Fase 5)
app.UseAdvancedHealthChecks();

// Executar seed data na inicialização
await DataSeederConfiguration.SeedDataAsync(app.Services, app.Logger);

app.Run();
