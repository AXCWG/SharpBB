using Microsoft.EntityFrameworkCore;

namespace SharpBB.Server.DbContexts;

public class ConfigurationSqliteDbContext : DbContext
{
    public DbSet<Settings> Settings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlite($"Data Source={Path.Join(INTERN_CONF_SINGLETONS.BaseDir, "conf.db")}");
}

[PrimaryKey(nameof(Key))]
public class Settings
{
    public required string Key { get; set; }
    public required string Value { get; set; }
}

public class DbTypeSettings
{
    public enum DbType
    {
        MySql,
        Sqlite
    }

    public required DbType Value { get; set; }

    public static implicit operator Settings(DbTypeSettings dbTypeSettings)
    {
        return new Settings
        {
            Key = "DbType", Value = dbTypeSettings.Value switch
            {
                DbType.MySql => "mysql", DbType.Sqlite => "sqlite", _ => throw new InvalidOperationException()
            }
        };
    }

    public static implicit operator DbTypeSettings(Settings dbSettings)
    {
        return dbSettings.Key == "DbType"
            ? new DbTypeSettings()
            {
                Value = dbSettings.Value switch
                {
                    "mysql" => DbType.MySql, "sqlite" => DbType.Sqlite, _ => throw new InvalidOperationException()
                }
            }
            : throw new NotSupportedException();
    }
}

public class MySqlConnectionStringSettings
{
    public required string ConnectionString { get; set; }

    public static implicit operator MySqlConnectionStringSettings(Settings dbSettings)
    {
        return dbSettings.Key == "MySqlConnectionString" ?  new()
        {
            ConnectionString = dbSettings.Value
        } :  throw new NotSupportedException();
    }

    public static implicit operator Settings(MySqlConnectionStringSettings dbSettings)
    {
        return new()
        {
            Key = "MySqlConnectionString", Value = dbSettings.ConnectionString
        }; 
    }
}

public static class DbSetExtensions
{
    extension(DbSet<Settings> dbSettings)
    {
        /// <summary>
        /// Does not save. 
        /// </summary>
        /// <returns></returns>
        public string? GetMySqlConnectionSetting()
        {
            if (dbSettings.Any(i=>i.Key == "MySqlConnectionString"))
            {
                return dbSettings.First(i => i.Key == "MySqlConnectionString").Value; 
            }

            return null; 
        }
        
        /// <summary>
        /// Does not save. 
        /// </summary>
        public void SetMySqlConnectionSetting(string @string)
        {
            if (dbSettings.Any(i=>i.Key == "MySqlConnectionString"))
            { 
                dbSettings.First(i => i.Key == "MySqlConnectionString").Value = @string;
                return; 
            }

            dbSettings.Add(new()
            {
                Key = "MySqlConnectionString", Value = @string
            }); 
        }

        public DbTypeSettings.DbType? GetDbTypeSettings()
        {
            if (dbSettings.Any(i => i.Key == "DbType"))
            {
                return ((DbTypeSettings)dbSettings.First(i => i.Key == "DbType")).Value;
            }
            return null;
        }

        public void SetDbTypeSettings(DbTypeSettings.DbType dbType)
        {
            if (dbSettings.Any(i => i.Key == "DbType"))
            {
                dbSettings.First(i => i.Key == "DbType").Value = dbType switch
                {
                    DbTypeSettings.DbType.Sqlite => "sqlite", DbTypeSettings.DbType.MySql => "mysql",
                    _ => throw new InvalidOperationException()
                };
                return; 
            }

            dbSettings.Add(new()
            {
                Key = "DbType", Value = dbType.ToString().ToLower()
            });
            
        }
    }
}