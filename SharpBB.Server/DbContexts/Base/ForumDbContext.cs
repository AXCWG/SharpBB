using Microsoft.EntityFrameworkCore;
using SharpBB.Server.DbContexts.Base.Models;

namespace SharpBB.Server.DbContexts.Base;

public class ForumDbContext : DbContext
{
    public virtual DbSet<Post> Posts { get; set; }
    public virtual DbSet<Board> Boards { get; set; }
    public virtual DbSet<BoardGroup> BoardGroups { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Announce> Announces { get; set; }
    public virtual DbSet<Message> Messages { get; set; }
    public virtual DbSet<UserPoint> UserPoints { get; set; }
    public virtual DbSet<UserBlock> UserBlocks { get; set; }
    public virtual DbSet<PostUserPostAllowed>  PostUserPostAllowed { get; set; }
    public virtual DbSet<PostUserPostBanned>  PostUserPostBanned { get; set; }
    public virtual DbSet<PostAllowedPointRange> PostAllowedPointRanges { get; set; }
    public virtual DbSet<BoardAllowedPointRange> BoardAllowedPointRanges { get; set; }
    public virtual DbSet<BoardUserPostAllowed> BoardUserPostAllowed { get; set; }
    public virtual DbSet<BoardUserPostBanned> BoardUserPostBanned { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Message>().HasOne(e => e.From).WithMany(e => e.Froms).HasForeignKey(e=>e.FromUuid); 
        modelBuilder.Entity<Message>().HasOne(e => e.To).WithMany(e => e.Tos).HasForeignKey(e=>e.ToUuid); 
        modelBuilder.Entity<UserBlock>().HasOne(e => e.By).WithMany(e => e.BlockIsBy).HasForeignKey(e=>e.ByUuid); 
        modelBuilder.Entity<UserBlock>().HasOne(e => e.To).WithMany(e => e.BlockIsTo).HasForeignKey(e=>e.ToUuid); 
        modelBuilder.Entity<Post>().HasOne(e=>e.Parent).WithMany(e=>e.Children).HasForeignKey(e=>e.ParentUuid);
        modelBuilder.Entity<Post>().HasOne(e=>e.TopParent).WithMany(e=>e.AllChildren).HasForeignKey(e=>e.TopParentUuid);
        modelBuilder.Entity<BoardGroup>().Property(i => i.Id).ValueGeneratedOnAdd(); 
    }
}