using Microsoft.EntityFrameworkCore;
using SharpBB.Server.DbContexts.Base;

namespace SharpBB.Server.DbContexts;

public class MySqlDbContext : ForumDbContext
{
    public override DbSet<Post> Posts { get; set; }
    public override DbSet<Board> Boards { get; set; }
    public override DbSet<User> Users { get; set; }
    public override DbSet<Announce> Announces { get; set; }
    public override DbSet<Message> Messages { get; set; }
    public override DbSet<Image> Images { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        using var conf = new ConfigurationSqliteDbContext();
        
        optionsBuilder.UseMySql(
           conf.Settings.GetMySqlConnectionSetting() ?? throw new InvalidOperationException("No connection string specified"),
            ServerVersion.AutoDetect(conf.Settings.GetMySqlConnectionSetting() ??  throw new InvalidOperationException("No connection string specified")));
    }
}