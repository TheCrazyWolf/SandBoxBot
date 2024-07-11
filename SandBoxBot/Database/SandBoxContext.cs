using Microsoft.EntityFrameworkCore;
using SandBoxBot.Models;

namespace SandBoxBot.Database;

public sealed class SandBoxContext : DbContext
{
    private static SandBoxContext? _blackBoxContext;

    public static SandBoxContext Instance => _blackBoxContext ??= new SandBoxContext();

    public SandBoxContext() => Database.MigrateAsync();
    
    public DbSet<Incident> Incidents { get; set; }
    public DbSet<BlackWord> BlackWords { get; set; }
    
    public DbSet<Account> Accounts { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source = LocalStorage.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Incident>()
            .HasOne(x => x.Account)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);
        
    }
}