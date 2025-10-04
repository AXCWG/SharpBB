using System.Linq.Expressions;
using System.Security.Cryptography.Xml;
using Microsoft.EntityFrameworkCore;

namespace SharpBB.Server.DbContexts;

public class ConfigurationSqliteDbContext : DbContext
{
    // ReSharper disable once InconsistentNaming
    private DbSet<Setting> settings { get; set; }

    public SettingsDbSet Settings => new(settings);

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlite($"Data Source={Path.Join(INTERN_CONF_SINGLETONS.BaseDir, "conf.db")}");
}

[PrimaryKey(nameof(Key))]
public class Setting
{
    public required string Key { get; set; }
    public required string Value { get; set; }
}

public enum DbType
{
    MySql,
    Sqlite
}

public static class DbQueryExtensions
{
    extension(DbSet<Setting> set)
    {
        public void AddIfNotExists(string key, string value)
        {
            if (set.Any(i => i.Key == key))
            {
                set.First(i => i.Key == key).Value = value;
                return;
            }

            set.Add(new()
            {
                Key = key, Value = value
            });
        }
    }
}

public class SettingsDbSet(DbSet<Setting> settings)
{
    private DbSet<Setting> SettingsInternal => settings;

    public string? MySqlConnectionString
    {
        get
        {
            if (SettingsInternal.Any(i => i.Key == "MySqlConnectionString"))
            {
                return SettingsInternal.First(i => i.Key == "MySqlConnectionString").Value;
            }

            return null;
        }
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (SettingsInternal.Any(i => i.Key == "MySqlConnectionString"))
            {
                SettingsInternal.First(i => i.Key == "MySqlConnectionString").Value = value;
                return;
            }

            SettingsInternal.Add(new()
            {
                Key = "MySqlConnectionString", Value = value
            });
        }
    }

    public DbType? DbType
    {
        get
        {
            if (SettingsInternal.Any(i => i.Key == "DbType"))
            {
                return SettingsInternal.First(i => i.Key == "DbType").Value switch
                {
                    "sqlite" => DbContexts.DbType.Sqlite, "mysql" => DbContexts.DbType.MySql,
                    _ => throw new InvalidOperationException()
                };
            }

            return null;
        }
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (SettingsInternal.Any(i => i.Key == "DbType"))
            {
                SettingsInternal.First(i => i.Key == "DbType").Value = value switch
                {
                    DbContexts.DbType.Sqlite => "sqlite", DbContexts.DbType.MySql => "mysql",
                    _ => throw new InvalidOperationException()
                };
                return;
            }

            SettingsInternal.Add(new()
            {
                Key = "DbType", Value = value.ToString()!.ToLower()
            });
        }
    }

    public byte[]? DefaultAvatar
    {
        get
        {
            if (SettingsInternal.Any(i => i.Key == "DefaultAvatar"))
            {
                return Convert.FromBase64String(SettingsInternal.First(i => i.Key == "DefaultAvatar").Value);
            }

            return null;
        }
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (SettingsInternal.Any(i => i.Key == "DefaultAvatar"))
            {
                SettingsInternal.First(i => i.Key == "DefaultAvatar").Value = Convert.ToBase64String(value);
                return;
            }

            SettingsInternal.Add(new()
            {
                Key = "DefaultAvatar", Value = Convert.ToBase64String(value)
            });
        }
    }

    public string? AdminContact
    {
        get => SettingsInternal.FirstOrDefault(i => i.Key == "AdminContact")?.Value;
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            SettingsInternal.AddIfNotExists("AdminContact", value);
        }
    }
}