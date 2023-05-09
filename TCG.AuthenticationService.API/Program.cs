using TCG.AuthenticationService.Application.Contracts;
using TCG.AuthenticationService.Persistence;
using TCG.AuthenticationService.Persistence.DependencyInjection;
using TCG.AuthenticationService.Persistence.ExternalsApi.KeycloakExternalApi.RepositoriesKeycloakExternalApi;
using TCG.CatalogService.Application;
using TCG.Common.Externals;
using TCG.Common.MySqlDb;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApplication();
builder.Services.AddSwaggerGen();
builder.Services.AddExternals<IKeycloakRepository, KeycloakRepository>();
builder.Services.AddPersistence<ServiceDbContext>(builder.Configuration);
builder.Services.AddMapper("User");
builder.Services.AddHttpClient();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
            .WithOrigins("http://localhost:8100") // Remplacez par l'URL de votre application Ionic
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});
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

app.Run();