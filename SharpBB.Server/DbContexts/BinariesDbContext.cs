using Microsoft.EntityFrameworkCore;
using SharpBB.Server.DbContexts.Base.Models;

namespace SharpBB.Server.DbContexts;

public class BinariesDbContext : DbContext
{
    public virtual DbSet<Binaries> Binaries { get; set;  }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlite($"Data Source={INTERN_CONF_SINGLETONS.BinarySqliteDir}");
}