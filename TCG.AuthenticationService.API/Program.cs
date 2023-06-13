using System.Configuration;
using TCG.AuthenticationService.Application.Contracts;
using TCG.AuthenticationService.Persistence;
using TCG.AuthenticationService.Persistence.DependencyInjection;
using TCG.AuthenticationService.Persistence.ExternalsApi.KeycloakExternalApi.RepositoriesKeycloakExternalApi;
using TCG.CatalogService.Application;
using TCG.Common.Authentification;
using TCG.Common.Externals;
using TCG.Common.Logging;
using TCG.Common.Middlewares.MiddlewareException;
using TCG.Common.MySqlDb;
using TCG.Common.Settings;

var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;

builder.Logging.ClearProviders();
builder.AddSerilogLogging();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApplication();
builder.Services.AddSwaggerGen();
builder.Services.AddExternals<IKeycloakRepository, KeycloakRepository>();
builder.Services.AddPersistence<ServiceDbContext>(builder.Configuration);
builder.Services.Configure<KeycloakSetting>(Configuration.GetSection("Keycloak"));
builder.Services.AddMapper("User");
builder.Services.AddHttpClient();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
            .WithOrigins("http://localhost:8100")
            .AllowAnyHeader()
            .AllowCredentials());
});

builder.Services.AddMassTransitWithRabbitMQ();
var app = builder.Build();
app.UseCors("CorsPolicy");



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.ConfigureCustomExceptionMiddleware();

app.Run();