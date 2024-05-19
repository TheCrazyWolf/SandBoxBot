using Microsoft.EntityFrameworkCore;
using SandBoxBot.Models;

namespace SandBoxBot.Database;

public sealed class BlackBoxContext : DbContext
{
    private static BlackBoxContext? _blackBoxContext;

    public static BlackBoxContext Instance => _blackBoxContext ??= new BlackBoxContext();

    public BlackBoxContext() => Database.MigrateAsync();
    
    public DbSet<Sentence> Sentences { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<BlackWord> BlackWords { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source = LocalStorage.db");
    }
}