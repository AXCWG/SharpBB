using Microsoft.EntityFrameworkCore;

namespace SharpBB.Server.DbContexts.Base;

public abstract class ForumDbContext : DbContext
{
    public abstract DbSet<Post> Posts { get; set; }
    public abstract DbSet<Board> Boards { get; set; }
    public abstract DbSet<User> Users { get; set; }
    public abstract DbSet<Announce> Announces { get; set; }
    public abstract DbSet<Message> Messages { get; set; }
    public abstract DbSet<Image> Images { get; set;  }
    
}