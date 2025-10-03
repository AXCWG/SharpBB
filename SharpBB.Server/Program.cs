using SharpBB.Server;
using SharpBB.Server.DbContexts;
using SharpBB.Server.DbContexts.Base;

// TODO Default Profile picture. 

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthorization();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


var app = builder.Build();

INTERN_CONF_SINGLETONS.BaseDir = app.Configuration["ConfigurationDirectory"];

Initialize();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.Run();

void Initialize()
{
    {
        var pre = new ConfigurationSqliteDbContext();
        pre.Database.EnsureCreated();
        pre.Dispose();
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
        Uuid = Guid.NewGuid().ToString(), Joined = DateTime.Now, Username = "Admin", Role = UserRole.Admin, Password = adminPassword.Sha256HexHashString()
    });
    dbContext.SaveChanges();
    
    Console.WriteLine("Initialization finished. Please restart SharpBB Server.");
    Console.ReadLine(); 
    Environment.Exit(0);
    return; 

    void SqliteInit()
    {
        using var conf = new ConfigurationSqliteDbContext();
        conf.Settings.SetDbTypeSettings(DbTypeSettings.DbType.Sqlite);
        conf.SaveChanges();
    }

    void MySqlInit()
    {
        Console.Write("Server: ");
        var server = Console.ReadLine()?.Trim();
        Console.Write("Username: ");
        var username = Console.ReadLine()?.Trim();
        Console.Write("Password: ");
        var password = Console.ReadLine()?.Trim();
        var connectionString = $"Server={server}; User={username}; Password={password};Database=SharpBB";
        using var conf = new ConfigurationSqliteDbContext();
        conf.Settings.SetDbTypeSettings(DbTypeSettings.DbType.MySql);
        conf.Settings.SetMySqlConnectionSetting(connectionString);
        conf.SaveChanges();
    }
}