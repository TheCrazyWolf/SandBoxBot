using Microsoft.EntityFrameworkCore;
using Raven.Storage.Models.Captchas;
using Raven.Storage.Models.Chats;
using Raven.Storage.Models.Countdowns;
using Raven.Storage.Models.Members;
using Raven.Storage.Models.Messages;

namespace Raven.Storage;

public sealed class RavenContext : DbContext
{
    public DbSet<Account> Accounts { get; private set; }
    public DbSet<MemberChat> MemberChats { get; private set; }
    public DbSet<ChatConfig> Chats { get; private set; }
    public DbSet<Captcha> Captchas { get; private set; }
    public DbSet<CountDown> CountDowns { get; private set; }
    public DbSet<MessageLog> Messages { get; private set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite("Data Source=app.db");
}