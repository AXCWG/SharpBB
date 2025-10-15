using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using SharpBB.Server;
using SharpBB.Server.DbContexts;
using SharpBB.Server.DbContexts.Base;
using SharpBB.Server.DbContexts.Base.Models;
using SharpBB.Server.Endpoints;
using WebPWrapper;


// TODO Programmatic procedural actions, something like:
/**
 * {
 *      action: enum Action => {
 *          Add, 
 *          Move, 
 *          Remove
 *      }
 *      ..Something else related to actiond. 
 * }
 * 
 * **/
var builder = WebApplication.CreateBuilder(args);

//builder.Services.Configure<JsonOptions>(o => o.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles); 
builder.Services.AddCors(o =>
{
    o.AddPolicy("All", p =>
    {
        p.WithOrigins("http://localhost:5173", "https://andyxie.cn:4000", "http://localhost:3000",
                "https://andyxie.cn:4001", "http://localhost:3001")
            .WithHeaders("Content-Type").AllowCredentials();
    });
});

builder.Services.AddSwaggerGen();

builder.Services.AddRateLimiter(_ =>
{
    _.AddPolicy<string>("UploadRateLimiting", context =>
    {
        var ip = context.Request.Host.Host;
        return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new()
        {
            PermitLimit = 6, AutoReplenishment = true, Window = TimeSpan.FromMinutes(10)
        });
    });
    _.OnRejected = (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        return ValueTask.CompletedTask;
    };
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(o =>
{
    o.IdleTimeout = TimeSpan.FromDays(7);
    o.Cookie.HttpOnly = true;
    o.Cookie.IsEssential = true;
    o.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

builder.Services.Configure<KestrelServerOptions>(o => o.Limits.MaxRequestBodySize = 1_000_000_000);


// Add services to the container.
builder.Services.AddAuthorization();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSpaStaticFiles(conf => conf.RootPath = "wwwroot");

var app = builder.Build();
app.UseRateLimiter();


INTERN_CONF_SINGLETONS.BaseDir = app.Configuration["ConfigurationDirectory"];
if (!INTERN_CONF_SINGLETONS.Initialized)
{
    Initialize();
}

// SPA
app.UseWhen(c => !c.Request.Path.StartsWithSegments("/api"), b =>
{
    b.UseSpa(spa =>
    {
        spa.Options.SourcePath = "../SharpBB.Client";
        if (app.Environment.IsDevelopment())
        {
            spa.UseProxyToSpaDevelopmentServer("http://localhost:3000");
        }
        else
        {
            b.UseSpaStaticFiles();
        }

    });
}); 





// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseCors("All");
app.UseAuthorization();
app.UseSession();

app.MapBbsEndpoints();

app.Run();

void Initialize()
{
    var images = new BinariesDbContext();
    images.Database.EnsureCreated(); 
    images.Dispose();
    if (INTERN_CONF_SINGLETONS.BaseDir is not null)
        Directory.CreateDirectory(INTERN_CONF_SINGLETONS.BaseDir);
    else
        throw new InvalidOperationException();
    {
        var pre = new ConfigurationSqliteDbContext();
        pre.Database.EnsureCreated();
        var arr = new MemoryStream();
        var s = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("SharpBB.Server.Assets.anonymous.webp")!;
        s.CopyTo(arr);
        pre.Settings.DefaultAvatar = arr.ToArray();
        using var arr2 = new MemoryStream(); 
        using var s2 = Assembly.GetExecutingAssembly().GetManifestResourceStream("SharpBB.Server.Assets.ForumIcon.webp")!;
        s2.CopyTo(arr2);
        pre.Settings.ForumIcon =  arr2.ToArray();
        pre.SaveChanges();
        pre.Dispose();
        arr.Dispose();
        s.Dispose();
    }
    using (var _ = new ConsoleHelper())
    {
        _.BlueWhite();
        Console.WriteLine("Welcome to SharpBB Server! ");
    }

    Console.WriteLine("Please specify the database type you would like to use. ");
    Console.WriteLine("1. MySQL");
    Console.WriteLine("2. Sqlite");

    DETECT_KEY:
    var key = Console.ReadKey(true);
    switch (key)
    {
        case { Key: ConsoleKey.D1 }:
            MySqlInit();
            break;
        case { Key: ConsoleKey.D2 }:
            SqliteInit();
            break;
        default:
            goto DETECT_KEY;
    }

    using var dbContext = INTERN_CONF_SINGLETONS.MainContext;
    ADMIN_PASS_SET:
    Console.WriteLine("The wizard now will set up an admin account for management. Please specify the password. ");
    var adminPassword = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(adminPassword))
    {
        Console.WriteLine("Password may not be null. ");
        goto ADMIN_PASS_SET;
    }

    Console.WriteLine("Please confirm your password. ");
    if (Console.ReadLine()?.Trim() != adminPassword)
    {
        Console.WriteLine("Confirmation failed. Please try again. ");
        goto ADMIN_PASS_SET;
    }


    dbContext.Database.EnsureCreated();
    dbContext.Users.Add(new User
    {
        Uuid = Guid.NewGuid().ToString(), Joined = DateTime.UtcNow, Username = "Admin", Role = User.UserRole.Admin,
        Password = adminPassword.Sha256HexHashString()
    });
    dbContext.SaveChanges();
    Console.WriteLine("""
        What would you like your forum to be called? This can be later edited in config. Leave the input empty to use the default. 
        """);
    var forumName = Console.ReadLine();
    Console.WriteLine("""
        What would you like to be your forum's description? This can be later edited in the config. Leave the input empty to not use any descriptions. 
        """);
    var forumDesc = Console.ReadLine();
    using (var conf = new ConfigurationSqliteDbContext())
    {
        conf.Settings.ForumName = forumName.IsNullOrWhiteSpace() ? "SharpBB" : forumName;
        conf.Settings.ForumDescription = forumDesc.NullIfEmpty();
        conf.SaveChanges(); 
    }

    Console.WriteLine("Initialization finished. Please restart SharpBB Server.");
    Environment.Exit(0);
    return;

    void SqliteInit()
    {
        using var conf = new ConfigurationSqliteDbContext();
        conf.Settings.DbType = DbType.Sqlite;
        conf.SaveChanges();
    }

    void MySqlInit()
    {
        START_MYSQL_INIT:
        Console.Write("Server: ");
        var server = Console.ReadLine()?.Trim();
        Console.Write("Username: ");
        var username = Console.ReadLine()?.Trim();
        Console.Write("Password: ");
        var password = Console.ReadLine()?.Trim();
        var connectionString = $"Server={server}; User={username}; Password={password};Database=SharpBB_Data";
        using (var conf = new ConfigurationSqliteDbContext())
        {
            conf.Settings.DbType = DbType.MySql;
            conf.Settings.MySqlConnectionString = connectionString;
            conf.SaveChanges();
        }
        
        var mysql = new MySqlDbContext();
        try
        {

            mysql.Database.EnsureCreated();
            Console.WriteLine("Database connection establishable.");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Database connection not establishable. Please try again. {e}");
            goto START_MYSQL_INIT;
        }
        finally
        {
            mysql.Dispose();
        }
        
        
        
    }
}