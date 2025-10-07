using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using SharpBB.Server.DbContexts.Base;
using SharpBB.Server.DbContexts.Base.Models;

namespace SharpBB.Server.DbContexts;

public class SqliteDbContext : ForumDbContext
{
    public override DbSet<Post> Posts { get; set; }
    public override DbSet<Board> Boards { get; set; }
    public override DbSet<User> Users { get; set; }
    public override DbSet<Announce> Announces { get; set;  }
    public override DbSet<Message> Messages { get; set; }
    public override DbSet<Image> Images { get; set; }
    public override DbSet<UserPoint> UserPoints { get; set; }
    public override DbSet<UserBlock> UserBlocks { get; set; }
    public override DbSet<PostUserPostAllowed> PostUserPostAllowed { get; set; }
    public override DbSet<PostUserPostBanned> PostUserPostBanned { get; set; }
    public override DbSet<PostAllowedPointRange> PostAllowedPointRanges { get; set; }
    public override DbSet<BoardAllowedPointRange> BoardAllowedPointRanges { get; set; }
    public override DbSet<BoardUserPostAllowed> BoardUserPostAllowed { get; set; }
    public override DbSet<BoardUserPostBanned> BoardUserPostBanned { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlite($"Data Source={INTERN_CONF_SINGLETONS.SqliteDir}");
}

