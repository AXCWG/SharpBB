using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using SharpBB.Server.DbContexts.Base;

namespace SharpBB.Server.DbContexts;

public class SqliteDbContext : ForumDbContext
{
    public override DbSet<Post> Posts { get; set; }
    public override DbSet<Board> Boards { get; set; }
    public override DbSet<User> Users { get; set; }
    public override DbSet<Announce> Announces { get; set;  }
    public override DbSet<Message> Messages { get; set; }
    public override DbSet<Image> Images { get; set; }
    public override DbSet<Point> Points { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlite($"Data Source={INTERN_CONF_SINGLETONS.SqliteDir}");
}

