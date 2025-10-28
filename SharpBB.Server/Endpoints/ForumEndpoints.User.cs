using ByteSizeLib;
using Microsoft.AspNetCore.Mvc;
using SharpBB.Server.DbContexts;
using SharpBB.Server.DbContexts.Base.Models;
using WebPWrapper.Encoder;

namespace SharpBB.Server.Endpoints;

public static partial class ForumEndpoints
{
    private enum UserApiRequestType
    {
        Username = 0,
        Email = 1,
        Profile = 2,
        Description = 3,
        Joined = 4,
        UserRole = 5,
        Posts = 6, 
        Uuid = 7
    }
    public class RegisterBody
    {
        public required string Username { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
    }
    public class LoginBody
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
    extension(WebApplication app)
    {
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
                        Type = 0,
                        MessageForReference = "Registration is disabled by administrator. ",
                    });
                }
                if (configuration.Settings.AllowEmptyPassword)
                {
                    if (string.IsNullOrWhiteSpace(body.Username))
                    {
                        return Results.BadRequest(new
                        {
                            Type = 1,
                            MessageForReference = "Username is required.",
                        });
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(body.Username) || string.IsNullOrWhiteSpace(body.Password))
                    {
                        return Results.BadRequest(new
                        {
                            Type = 2,
                            MessageForReference = "Username and password are required.",
                        });
                    }
                }

                if (configuration.Settings.EnforceEmail)
                {
                    if (string.IsNullOrWhiteSpace(body.Email))
                    {
                        return Results.BadRequest(new
                        {
                            Type = 3,
                            MessageForReference = "Email is required.",
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
                        Password = body.Password?.Sha256HexHashString(),
                        Uuid = uuid,
                        Role = User.UserRole.People,
                        Email = body.Email,
                        Joined = DateTime.UtcNow
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
                    using var conf = new ConfigurationSqliteDbContext();
                    
                    if (body.Username.Contains('@') && conf.Settings.EnableLoginWithEmail)
                    {
                        var users = db.Users.Where(i =>
                            i.Email == body.Username && i.Password == body.Password.Sha256HexHashString()).ToList();
                        switch (users.Count)
                        {
                            case 0:
                                return Results.NotFound("Username, email or password is incorrect. ");
                            case 1:
                                context.Session.SetString("uuid", users[0].Uuid); 
                                return Results.Ok(users[0].Uuid);
                            default:
                                return Results.Conflict(users.Select(i => i.Username)); 
                        }
                    }
                    var user = db.Users.FirstOrDefault(i =>
                        i.Username == body.Username && i.Password == body.Password.Sha256HexHashString());
                    if (user == null)
                    {
                        return Results.NotFound("Username or password is incorrect");
                    }
                    context.Session.SetString("uuid", user.Uuid);
                    return Results.Ok(user.Uuid);

                }
                catch (Exception e)
                {
                    return GeneralHandler(e);
                }
            });

            userApi.MapGet("userapi", (HttpContext context, [FromQuery] UserApiRequestType? requestType) =>
            {
                using var db = INTERN_CONF_SINGLETONS.MainContext;
                var sessionUuid = context.Session.GetString("uuid");
                if (sessionUuid.IsNullOrWhiteSpace() || !db.Users.Any(i => i.Uuid == sessionUuid))
                {
                    return Results.NotFound();
                }

                if (requestType is null)
                {
                    return Results.Ok("Ok, but no request type is specified. "); 
                }
                switch (requestType)
                {
                    case UserApiRequestType.Username:
                        return Results.Ok(db.Users.FirstOrDefault(i => i.Uuid == sessionUuid)?.Username);
                    case UserApiRequestType.Email:
                        return Results.Ok(db.Users.FirstOrDefault(i => i.Uuid == sessionUuid)?.Email);
                    case UserApiRequestType.Description:
                        return Results.Ok(db.Users.FirstOrDefault(i => i.Uuid == sessionUuid)?.Description);
                    case UserApiRequestType.Joined:
                        return Results.Ok(db.Users.FirstOrDefault(i => i.Uuid == sessionUuid)?.Joined);
                    case UserApiRequestType.UserRole:
                        return Results.Ok(db.Users.FirstOrDefault(i => i.Uuid == sessionUuid)?.Role);
                    case UserApiRequestType.Posts:
                        return Results.Ok(db.Posts.Where(i => i.ByUuid == sessionUuid).ToList());
                    case UserApiRequestType.Profile:
                        return Results.Ok(db.Users.FirstOrDefault(i => i.Uuid == sessionUuid)?.Profile);
                    case UserApiRequestType.Uuid:
                        return Results.Ok(sessionUuid); 
                }


                return Results.BadRequest();
            });
            userApi.MapGet("getnamefast", (string uuid) =>
            {
                using var db = INTERN_CONF_SINGLETONS.MainContext;
                return Results.Text(db.Users.FirstOrDefault(i => i.Uuid == uuid)?.Username); 
            }); 

            userApi.MapPost("changeAvatar", (HttpContext context, [FromForm] IFormFile image) =>
            {
                var sessionUuid = context.Session.GetString("uuid");
                using var db = INTERN_CONF_SINGLETONS.MainContext;
                if (!db.Users.Any(i => i.Uuid == sessionUuid))
                {
                    return Results.NotFound();
                }
                if (!image.ContentType.StartsWith("image/"))
                {
                    return Results.StatusCode(StatusCodes.Status415UnsupportedMediaType);
                }
                using var mStream = new MemoryStream();
                image.CopyTo(mStream);

                try
                {
                    var cwebp = new WebPEncoderBuilder();
                    var encoder = cwebp
                        .Resize(500, 0)
                        .CompressionConfig(x => x.Lossy(y => y.Size((int)(15 * ByteSize.BytesInKiloByte))))
                        .Build();
                    using var outputStream = new MemoryStream();
                    mStream.Position = 0;
                    encoder.Encode(mStream, outputStream);
                    db.Users.FirstOrDefault(i => i.Uuid == sessionUuid)?.Profile = outputStream.ToArray();
                    db.SaveChanges();
                    return Results.Ok();
                }
                catch (Exception e)
                {
                    return GeneralHandler(e);
                }

            }).DisableAntiforgery();
            return app;
        }
    }

    
}