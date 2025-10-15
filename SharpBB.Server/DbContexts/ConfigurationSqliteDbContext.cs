using System.Linq.Expressions;
using System.Security.Cryptography.Xml;
using System.Text.Json;
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
    public string? Key { get; set; }
    public string? Value { get; set; }
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
        public void AddIfNotExists(string key, string? value)
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
        get => SettingsInternal.FirstOrDefault(i => i.Key == "MySqlConnectionString")?.Value;
        set => SettingsInternal.AddIfNotExists("MySqlConnectionString", value);
    }
    public DbType? DbType
    {
        get =>
            SettingsInternal.FirstOrDefault(i => i.Key == "DbType")?.Value switch
            {
                "sqlite" => DbContexts.DbType.Sqlite, "mysql" => DbContexts.DbType.MySql, null => null,
                _ => throw new InvalidOperationException()
            };
        set => SettingsInternal.AddIfNotExists("DbType", value.ToString()!.ToLower());
    }

    public byte[]? DefaultAvatar
    {
        get
        {
            var avatar = SettingsInternal.FirstOrDefault(i => i.Key == "DefaultAvatar")?.Value;
            return avatar != null ? Convert.FromBase64String(avatar) : null;
        }
        set
        {
            if (value == null)
            {
                SettingsInternal.AddIfNotExists("DefaultAvatar", null);
                return;
            }

            SettingsInternal.AddIfNotExists("DefaultAvatar", Convert.ToBase64String(value));
        }
    }

    public byte[]? ForumIcon
    {
        get
        {
            var icon = SettingsInternal.FirstOrDefault(i => i.Key == "ForumIcon")?.Value; 
            return icon != null ? Convert.FromBase64String(icon) : null;
        }
        set
        {
            if (value is null)
            {
                SettingsInternal.AddIfNotExists("ForumIcon", null);
                return; 
            }
            SettingsInternal.AddIfNotExists("ForumIcon", Convert.ToBase64String(value));
        }
    }

    public string? AdminContact
    {
        get => SettingsInternal.FirstOrDefault(i => i.Key == "AdminContact")?.Value;
        set => SettingsInternal.AddIfNotExists("AdminContact", value);
    }

    public bool AllowAnonymousUser
    {
        get => SettingsInternal.FirstOrDefault(i => i.Key == "AllowAnonymousUser")?.Value?.ToBoolean() ?? false;
        set => SettingsInternal.AddIfNotExists("AllowAnonymousUser", value.ToStringStandard());
    }
    public bool AllowAnonymousRead
    {
        get => (SettingsInternal.FirstOrDefault(i => i.Key == "AllowAnonymousRead")?.Value?.ToBoolean() ?? false) && AllowAnonymousUser;
        set => SettingsInternal.AddIfNotExists("AllowAnonymousRead", value.ToStringStandard());
    }

    public bool AllowAnonymousPost
    {
        get => (SettingsInternal.FirstOrDefault(i => i.Key == "AllowAnonymousPost")?.Value?.ToBoolean() ?? false) && AllowAnonymousUser;
        set => SettingsInternal.AddIfNotExists("AllowAnonymousPost", value.ToStringStandard());
    }

    public bool AllowAnonymousReply
    {
        get => (SettingsInternal.FirstOrDefault(i => i.Key == "AllowAnonymousReply")?.Value?.ToBoolean() ?? false) && AllowAnonymousUser;
        set => SettingsInternal.AddIfNotExists("AllowAnonymousReply", value.ToStringStandard());
    }

    public bool AllowAnonymousMessaging
    {
        get => (SettingsInternal.FirstOrDefault(i => i.Key == "AllowAnonymousMessages")?.Value?.ToBoolean() ?? false) && AllowAnonymousUser;
        set => SettingsInternal.AddIfNotExists("AllowAnonymousMessages", value.ToStringStandard());
    }

    public bool AllowAnonymousImages
    {
        get => (SettingsInternal.FirstOrDefault(i => i.Key == "AllowAnonymousImages")?.Value?.ToBoolean() ?? false) && AllowAnonymousUser;
        set => SettingsInternal.AddIfNotExists("AllowAnonymousImages", value.ToStringStandard());
    }

    public bool AllowUserCreatingBoards
    {
        get => SettingsInternal.FirstOrDefault(i=>i.Key == "AllowUserCreatingBoards")?.Value?.ToBoolean() ?? false;
        set => SettingsInternal.AddIfNotExists("AllowUserCreatingBoards", value.ToStringStandard());
    }

    public bool AllowPromoteSubAdmins
    {
        get => (SettingsInternal).FirstOrDefault(i=>i.Key == "AllowPromoteSubAdmins")?.Value?.ToBoolean() ?? false;
        set => SettingsInternal.AddIfNotExists("AllowPromoteSubAdmins", value.ToStringStandard());
    }

    public bool AllowEmptyPassword
    {
        get => SettingsInternal.FirstOrDefault(i => i.Key == "AllowEmptyPassword")?.Value?.ToBoolean() ?? false;
        set => SettingsInternal.AddIfNotExists("AllowEmptyPassword", value.ToStringStandard());
    }
    public bool EnableRegistration
    {
        get => SettingsInternal.FirstOrDefault(i => i.Key == "EnableRegistration")?.Value?.ToBoolean() ?? false;
        set => SettingsInternal.AddIfNotExists("EnableRegistration", value.ToStringStandard());
    }

    public bool EnableLoginWithEmail
    {
        get => SettingsInternal.FirstOrDefault(i=>i.Key == "EnableLoginWithEmail")?.Value?.ToBoolean() ?? false;
        set => SettingsInternal.AddIfNotExists("EnableLoginWithEmail", value.ToStringStandard());
    }
    public bool EnforceEmail
    {
        get => SettingsInternal.FirstOrDefault(i => i.Key == "EnforceEmail")?.Value?.ToBoolean() ?? false;
        set => SettingsInternal.AddIfNotExists("EnforceEmail", value.ToStringStandard());
    }
    public string? ForumName
    {
        get => SettingsInternal.FirstOrDefault(i => i.Key == "ForumName")?.Value; 
        set => SettingsInternal.AddIfNotExists("ForumName", value);
    }
    public string? ForumDescription
    {
        get => SettingsInternal.FirstOrDefault(i => i.Key == "ForumDescription")?.Value;
        set => SettingsInternal.AddIfNotExists("ForumDescription", value); 
    }
}