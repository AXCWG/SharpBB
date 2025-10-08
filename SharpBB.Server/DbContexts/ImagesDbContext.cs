using Microsoft.EntityFrameworkCore;
using SharpBB.Server.DbContexts.Base.Models;

namespace SharpBB.Server.DbContexts;

public class ImagesDbContext : DbContext
{
    public virtual DbSet<Image> Images { get; set;  }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlite($"Data Source={INTERN_CONF_SINGLETONS.ImageSqliteDir}");
}