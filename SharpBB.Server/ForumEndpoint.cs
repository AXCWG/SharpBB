using Microsoft.AspNetCore.Mvc;
using SharpBB.Server.DbContexts;
using SharpBB.Server.DbContexts.Base;

namespace SharpBB.Server;

public static class ForumEndpoint
{
    // TODO Check Username / Password constraints. 
    private class RegisterBody
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
    }
    private class LoginBody
    {
        public string? Username{ get; set;  }
        public string? Password { get; set;  }
    }

    public static IResult GeneralHandler(Exception e)
    {
        using var conf = new ConfigurationSqliteDbContext();
        var message = conf.Settings.AdminContact != null
            ? $"An error occured while registering user: {e.Message}. Please contact the server admin through {conf.Settings.AdminContact}"
            : $"An error occured while registering user: {e.Message}. Please contact the server admin.";
        return Results.InternalServerError(message);
    }
    extension(WebApplication app)
    {
        public void MapBbsEndpoints()
        {
            var userApi = app.MapGroup("/api/bbs/user");


            userApi.MapPost("register", ([FromBody] RegisterBody body, HttpContext context) =>
            {
                // TODO Here. 
                if (string.IsNullOrWhiteSpace(body.Username) || string.IsNullOrWhiteSpace(body.Password))
                {
                    return Results.BadRequest("Username or password is required");
                }

                try
                {
                    using var db = INTERN_CONF_SINGLETONS.MainContext;
                    var uuid = Guid.NewGuid().ToString();
                    db.Users.Add(new()
                    {
                        Username = body.Username,
                        Password = body.Password.Sha256HexHashString(), Uuid = uuid,
                        Role = UserRole.People, Email = body.Email, Joined = DateTime.Now
                    });
                    db.SaveChanges();
                    context.Session.SetString(nameof(uuid), uuid);
                    return Results.Ok();
                }
                catch (Exception e)
                {
                    return GeneralHandler(e); 
                }
            });

       
            userApi.MapPost("login", ([FromBody] LoginBody body, HttpContext context) =>
            {
                // TODO Here. 
                if (string.IsNullOrWhiteSpace(body.Username) || string.IsNullOrWhiteSpace(body.Password))
                {
                    return Results.BadRequest(); 
                }

                try
                {
                    using var db = INTERN_CONF_SINGLETONS.MainContext;
                    var user = db.Users.FirstOrDefault(i =>
                        i.Username == body.Username && i.Password == body.Password.Sha256HexHashString());
                    if (user == null)
                    {
                        return Results.NotFound("Username or password is incorrect");
                    }

                    context.Session.SetString("uuid", user.Uuid);
                    return Results.Ok();
                }
                catch (Exception e)
                {
                    return GeneralHandler(e); 
                }
            }); 
        }
    }
}