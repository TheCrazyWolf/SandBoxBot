using Microsoft.EntityFrameworkCore;
using SandBox.Models.Blackbox;
using SandBox.Models.Common;
using SandBox.Models.Events;
using SandBox.Models.Telegram;

namespace SandBox.Advanced.Database;

public class SandBoxContext : DbContext
{
    public DbSet<BlackWord> BlackWords { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<EventJoined> EventsJoined { get; set; }
    public DbSet<EventContent> EventsContent { get; set; }
    public DbSet<Captcha> Captchas { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source = LocalStorage.db");
    }
}