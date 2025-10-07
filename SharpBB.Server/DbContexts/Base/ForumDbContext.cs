using Microsoft.EntityFrameworkCore;
using SharpBB.Server.DbContexts.Base.Models;

namespace SharpBB.Server.DbContexts.Base;

public abstract class ForumDbContext : DbContext
{
    public abstract DbSet<Post> Posts { get; set; }
    public abstract DbSet<Board> Boards { get; set; }
    public abstract DbSet<User> Users { get; set; }
    public abstract DbSet<Announce> Announces { get; set; }
    public abstract DbSet<Message> Messages { get; set; }
    public abstract DbSet<Image> Images { get; set;  }
    public abstract DbSet<UserPoint> UserPoints { get; set; }
    public abstract DbSet<UserBlock> UserBlocks { get; set; }
    public abstract DbSet<PostUserPostAllowed>  PostUserPostAllowed { get; set; }
    public abstract DbSet<PostUserPostBanned>  PostUserPostBanned { get; set; }
    public abstract DbSet<PostAllowedPointRange> PostAllowedPointRanges { get; set; }
    public abstract DbSet<BoardAllowedPointRange> BoardAllowedPointRanges { get; set; }
    public abstract DbSet<BoardUserPostAllowed> BoardUserPostAllowed { get; set; }
    public abstract DbSet<BoardUserPostBanned> BoardUserPostBanned { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Message>().HasOne(e => e.From).WithMany(e => e.Froms).HasForeignKey(e=>e.FromUuid); 
        modelBuilder.Entity<Message>().HasOne(e => e.To).WithMany(e => e.Tos).HasForeignKey(e=>e.ToUuid); 
        modelBuilder.Entity<UserBlock>().HasOne(e => e.By).WithMany(e => e.BlockIsBy).HasForeignKey(e=>e.ByUuid); 
        modelBuilder.Entity<UserBlock>().HasOne(e => e.To).WithMany(e => e.BlockIsTo).HasForeignKey(e=>e.ToUuid); 
    }
}