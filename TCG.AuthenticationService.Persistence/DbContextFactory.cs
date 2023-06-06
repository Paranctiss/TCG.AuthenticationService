using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using TCG.AuthenticationService.Persistence;
using TCG.Common.Settings;

namespace TCG.PostService.Persistence;

public class DbContextFactory : IDesignTimeDbContextFactory<ServiceDbContext>
{
    public ServiceDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory() + "/../TCG.AuthenticationService.API")
            .AddJsonFile("appsettings.Development.json")
            .Build();
    
        var databaseSettings = configuration.GetSection("MySqlDatabaseSettings").Get<SqlDbSettings>();

        var optionsBuilder = new DbContextOptionsBuilder<ServiceDbContext>();
        optionsBuilder.UseMySQL(databaseSettings.ConnectionString);

        return new ServiceDbContext(optionsBuilder.Options, configuration);
    }

}