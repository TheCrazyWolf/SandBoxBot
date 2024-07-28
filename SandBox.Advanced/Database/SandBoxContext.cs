using Microsoft.EntityFrameworkCore;
using SandBox.Models.Blackbox;
using SandBox.Models.Common;
using SandBox.Models.Events;
using SandBox.Models.Telegram;

namespace SandBox.Advanced.Database;

public sealed class SandBoxContext : DbContext
{
    public SandBoxContext() => Database.MigrateAsync();
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<EventJoined> EventsJoined { get; set; }
    public DbSet<EventContent> EventsContent { get; set; }
    public DbSet<Captcha> Captchas { get; set; }
    public DbSet<ChatProps> Chats { get; set; }
    public DbSet<MemberInChat> MembersInChats { get; set; } 
    public DbSet<Question> Questions { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source = LocalStorage.db");
    }
}