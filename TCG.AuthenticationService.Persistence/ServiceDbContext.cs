using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TCG.AuthenticationService.Domain;

namespace TCG.AuthenticationService.Persistence;

public class ServiceDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    public ServiceDbContext()
    {
        
    }
    public ServiceDbContext(DbContextOptions<ServiceDbContext> options, IConfiguration configuration) : base(options)
    {
        Database.EnsureCreated();
        _configuration = configuration;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseMySQL(
                "Host=localhost;Port=3307;Database=user_database;Username=myuser2;Password=mypassword2");
        }
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<UserState> UserStates { get; set; }
    
    public Task<int> SaveChangesAsync()
    {
        return base.SaveChangesAsync();
    }

    public Task Migrate()
    {
        return base.Database.MigrateAsync();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasOne(p => p.UserState)
            .WithMany(s => s.Users)
            .HasForeignKey(p => p.UserStateId)
            .IsRequired();
        
        modelBuilder.Entity<User>()
            .HasOne(p => p.Country)
            .WithMany(s => s.Users)
            .HasForeignKey(p => p.CountryId)
            .IsRequired();
    }
}