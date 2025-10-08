using Microsoft.AspNetCore.Mvc;
using SharpBB.Server.DbContexts;
using SharpBB.Server.DbContexts.Base;
using SharpBB.Server.DbContexts.Base.Models;

namespace SharpBB.Server;

public static class ForumEndpoint
{
    private class RegisterBody
    {
        public required string Username { get; set; }
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
        return Results.InternalServerError(new{e.Message, conf.Settings.AdminContact});
    }
    extension(WebApplication app)
    {
        public WebApplication MapBbsEndpoints()
        {
            app.MapBbsUserEndpoints().MapBbsPostEndpoints().MapBbsStatusEndpoints();
            return app; 
        }

        public WebApplication MapBbsUserEndpoints()
        {
            var userApi = app.MapGroup("/api/bbs/user");


            userApi.MapPost("register", ([FromBody] RegisterBody body, HttpContext context) =>
            {
                var configuration = new ConfigurationSqliteDbContext();
                if (!configuration.Settings.EnableRegistration)
                {
                    return Results.BadRequest(new
                    {
                        Type=0, MessageForReference="Registration is disabled by administrator. ",
                    }); 
                }
                if (configuration.Settings.AllowEmptyPassword)
                {
                    if (string.IsNullOrWhiteSpace(body.Username))
                    {
                        return Results.BadRequest(new
                        {
                            Type=1, MessageForReference="Username is required.",
                        });
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(body.Username) || string.IsNullOrWhiteSpace(body.Password))
                    {
                        return Results.BadRequest(new
                        {
                            Type=2, MessageForReference="Username and password are required.",
                        }); 
                    }
                }

                if (configuration.Settings.EnforceEmail)
                {
                    if (string.IsNullOrWhiteSpace(body.Email))
                    {
                        return Results.BadRequest(new
                        {
                            Type = 3, MessageForReference = "Email is required.",
                        });
                    }
                }
                configuration.Dispose();
                try
                {
                    using var db = INTERN_CONF_SINGLETONS.MainContext;
                    var uuid = Guid.NewGuid().ToString();
                    db.Users.Add(new()
                    {
                        Username = body.Username,
                        Password = body.Password?.Sha256HexHashString(), Uuid = uuid,
                        Role = User.UserRole.People, Email = body.Email, Joined = DateTime.Now
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
            return app; 
        }

        public WebApplication MapBbsPostEndpoints()
        {
            var postApi = app.MapGroup("/api/bbs/post");
            return app; 
        }

        public WebApplication MapBbsStatusEndpoints()
        {
            app.MapGet("/api/bbs/conf", () =>
            {
                using var conf = new ConfigurationSqliteDbContext();
                return Results.Ok(new
                {
                    conf.Settings.AdminContact,
                    conf.Settings.AllowAnonymousUser,
                    conf.Settings.AllowAnonymousRead,
                    conf.Settings.AllowAnonymousPost,
                    conf.Settings.AllowAnonymousReply, 
                    conf.Settings.AllowAnonymousMessaging,
                    conf.Settings.AllowAnonymousImages,
                    conf.Settings.AllowUserCreatingBoards,
                    conf.Settings.AllowPromoteSubAdmins, 
                    conf.Settings.EnableRegistration,
                    conf.Settings.AllowEmptyPassword,
                    conf.Settings.EnforceEmail,
                });
            });
            app.MapGet("/api/bbs/numerical", () =>
            {
                using var db = INTERN_CONF_SINGLETONS.MainContext;
                using var imagesDb = new ImagesDbContext(); 
                return Results.Ok(new
                {
                    UserCount = db.Users.Count(),
                    BoardCount = db.Boards.Count(),
                    PostCount = db.Posts.Count(),
                    MessageCount = db.Messages.Count(),
                    ImageCount = imagesDb.Images.Count(),
                });
            }); 
            return app; 
        }
    }
}