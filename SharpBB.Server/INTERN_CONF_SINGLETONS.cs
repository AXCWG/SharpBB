using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SharpBB.Server.DbContexts;
using SharpBB.Server.DbContexts.Base;
using SQLitePCL;

namespace SharpBB.Server;





// ReSharper disable once InconsistentNaming

public static class INTERN_CONF_SINGLETONS
{


    public static bool Initialized { get; set; } 
    public static string? BaseDir
    {
        get
        {
            if (!Directory.Exists(field))
            {
                throw new DirectoryNotFoundException(field);
            }

            return field; 
        }
        set
        {
            if (value == null)
            {
                return; 
            }
            if (!Directory.Exists(value))
            {
                Initialized = false;
                Directory.CreateDirectory(value);
            }

            field = value; 
        }
    }

    public static string SqliteDir
    {
        get
        {
            if (BaseDir == null)
            {
                throw new NullReferenceException("Please specify valid base directory before accessing any members inside INTERN_CONF. "); 
            }

            var dir = Path.Join(BaseDir, "data");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return Path.Join(dir, "bbs_data.db");
        }
    }

    public static ForumDbContext MainContext
    {
        get
        {
            using var conf = new ConfigurationSqliteDbContext(); 
            switch (conf.Settings.GetDbTypeSettings())
            {
                case DbTypeSettings.DbType.Sqlite:
                    return new SqliteDbContext();
                case DbTypeSettings.DbType.MySql:
                    return new MySqlDbContext();
                default:
                    throw new NotImplementedException(); 
            }
            
        }
        
    }
}