using ByteSizeLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using SharpBB.Server.DbContexts;
using SharpBB.Server.DbContexts.Base;
using SharpBB.Server.DbContexts.Base.Models;
using SharpBB.Server.DbContexts.Base.Models.DTOs;
using System;
using System.Net.Mime;
using WebPWrapper.Encoder;

namespace SharpBB.Server;

public static class ForumEndpoint
{
    public class RegisterBody
    {
        public required string Username { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
    }
    public class LoginBody
    {
        public string? Username{ get; set;  }
        public string? Password { get; set;  }
    }


    private enum UserApiRequestType
    {
        Username=0, 
        Email=1, 
        Profile=2, 
        Description=3, 
        Joined=4, 
        UserRole=5, 
        Posts=6
    }

    private enum GetPostOrderType
    {
        DateTime, Alphabetical
    }

    private class PostPostBody
    {
        public required string Title { get; set;  }
        public required string Content { get; set;  }
        public required string BoardUuid { get; set; }
        public string? Parent { get; set;  }
    }



    public static IResult GeneralHandler(Exception e)
    {
        using var conf = new ConfigurationSqliteDbContext();
        return Results.InternalServerError(new{Message=e.InnerException is null ? e.Message : e.InnerException.Message, conf.Settings.AdminContact});
    }
    extension(WebApplication app)
    {
        public WebApplication MapBbsEndpoints()
        {
            app.MapBbsUserEndpoints().MapBbsPostEndpoints().MapBbsStatusEndpoints().MapBbsImageEndpoints();
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
                        Role = User.UserRole.People, Email = body.Email, Joined = DateTime.UtcNow
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

            userApi.MapGet("userapi", (HttpContext context, [FromQuery] UserApiRequestType requestType, string? uuid) =>
            {
                using var db = INTERN_CONF_SINGLETONS.MainContext;
                if (!string.IsNullOrWhiteSpace(uuid))
                {
                    return Results.Ok(); 
                }
                var sessionUuid = context.Session.GetString("uuid");
                if (!db.Users.Any(i => i.Uuid == sessionUuid))
                {
                    return Results.NotFound(); 
                }
                switch (requestType)
                {
                    case UserApiRequestType.Username:
                        return Results.Ok(db.Users.FirstOrDefault(i=>i.Uuid == sessionUuid)?.Username);
                    case UserApiRequestType.Email:
                        return Results.Ok(db.Users.FirstOrDefault(i=>i.Uuid == sessionUuid)?.Email);
                    case UserApiRequestType.Description:
                        return Results.Ok(db.Users.FirstOrDefault(i=>i.Uuid==sessionUuid)?.Description);
                    case UserApiRequestType.Joined:
                        return Results.Ok(db.Users.FirstOrDefault(i=>i.Uuid==sessionUuid)?.Joined);
                    case UserApiRequestType.UserRole:
                        return Results.Ok(db.Users.FirstOrDefault(i=>i.Uuid==sessionUuid)?.Role);
                    case UserApiRequestType.Posts:
                        return Results.Ok(db.Posts.Where(i=>i.ByUuid == sessionUuid).ToList()); 
                    case UserApiRequestType.Profile:
                        return Results.Ok(db.Users.FirstOrDefault(i=>i.Uuid==sessionUuid)?.Profile);
                }
                

                return Results.BadRequest(); 
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
        
        public WebApplication MapBbsPostEndpoints()
        {
            var postApi = app.MapGroup("/api/bbs/post");
            postApi.MapGet("get", (HttpContext context, int? from, int? count, GetPostOrderType orderType, string? boardUuid) =>
            {
                // I'm so confused. 
                using var db = INTERN_CONF_SINGLETONS.MainContext;
                if (string.IsNullOrWhiteSpace(boardUuid))
                {
                    var query = count.HasValue ? db.Posts.OrderByDescending(i => i.DateTime).Skip(from ?? 0).Take( count.Value ).ToList() : db.Posts.OrderByDescending(i=>i.DateTime).Skip(from ?? 0).ToList();
                    
                    return Results.Ok(query.Select(i=>(GetPostPayload)i));
                }
                return Results.Ok((count.HasValue
                    ? orderType switch
                    {
                        GetPostOrderType.DateTime => db.Posts.Where(i => i.BoardUuid == boardUuid).OrderByDescending(i => i.DateTime).Skip(from ?? 0)
                            .Take(count.Value).ToList(), 
                        GetPostOrderType.Alphabetical=>db.Posts.Where(i => i.BoardUuid == boardUuid).OrderByDescending(i => i.Title).Skip(from ?? 0)
                            .Take(count.Value).ToList(),
                        _ => throw new ArgumentOutOfRangeException(nameof(orderType), orderType, null)
                    }
                    : orderType switch
                    {
                        GetPostOrderType.DateTime => db.Posts.Where(i => i.BoardUuid == boardUuid).OrderByDescending(i => i.DateTime).Skip(from ?? 0)
                            .ToList(), 
                        GetPostOrderType.Alphabetical=>db.Posts.Where(i => i.BoardUuid == boardUuid).OrderByDescending(i => i.Title).Skip(from ?? 0)
                            .ToList(),
                        _ => throw new ArgumentOutOfRangeException(nameof(orderType), orderType, null)
                    }).Select(i=>(GetPostPayload)i));
            });
            postApi.MapPost("post", (HttpContext context, [FromForm] PostPostBody body) =>
            {
                using var db = INTERN_CONF_SINGLETONS.MainContext;
                using var conf = new ConfigurationSqliteDbContext();
                var sessionUuid = context.Session.GetString("uuid");

                if (!conf.Settings.AllowAnonymousPost && sessionUuid.IsNullOrWhiteSpace())
                {
                    return Results.BadRequest();
                }
                try
                {
                    db.Posts.Add(new()
                    {
                        BoardUuid = body.BoardUuid,
                        Board = db.Boards.FirstOrDefault(i => i.Uuid == body.BoardUuid)!,
                        DateTime = DateTime.UtcNow,
                        Uuid = Guid.NewGuid().ToString(),
                        Content = body.Content,
                        Title = body.Title,
                        ByUuid = sessionUuid, By = db.Users.FirstOrDefault(i => i.Uuid == sessionUuid),
                        ParentUuid = body.Parent, Parent = db.Posts.FirstOrDefault(i => i.Uuid == body.Parent)
                    });
                    db.SaveChanges();
                    return Results.Ok();
                }
                catch (Exception e)
                {
                    return GeneralHandler(e);
                }
                    
               
            }); 
            return app; 
        }

        public WebApplication MapBbsImageEndpoints()
        {
            var imageApis = app.MapGroup("api/bbs/binary");
            imageApis.MapPost("post", ([FromForm]IFormFile binaryFile) =>
            {
                var uuid = Guid.NewGuid().ToString();
                using var mStream = new MemoryStream();
                binaryFile.CopyTo(mStream);
                switch (binaryFile.ContentType)
                {
                    case var ct when ct.StartsWith("image/"):
                        try
                        {
                            var cwebp = new WebPEncoderBuilder();
                            var encoder = cwebp
                                .CompressionConfig(x => x.Lossy(y => y.Size((int)(400 * ByteSize.BytesInKiloByte))))
                                .Build();
                            using var outputStream = new MemoryStream();
                            mStream.Position = 0;
                            encoder.Encode(mStream, outputStream);
                            using var binariesDbContext = new BinariesDbContext();
                            binariesDbContext.Binaries.Add(new()
                            {
                                Uuid = uuid, Content = outputStream.ToArray(), MimeType = "image/webp", FileName = binaryFile.FileName.Split(".")[0]+".webp"
                            });
                            binariesDbContext.SaveChanges();
                            return Results.Ok(uuid); 
                        }
                        catch (Exception e)
                        {
                            return GeneralHandler(e);
                        }

                }
                try
                {
                    using var binariesDbContext = new BinariesDbContext();
                    binariesDbContext.Binaries.Add(new()
                    {
                        Uuid = uuid, Content = mStream.ToArray(), MimeType = binaryFile.ContentType, FileName = binaryFile.FileName
                    });
                    binariesDbContext.SaveChanges();
                }
                catch (Exception e)
                {
                    return GeneralHandler(e); 
                }
                return Results.Ok(uuid);
            }).DisableAntiforgery().WithFormOptions(multipartBodyLengthLimit:10*ByteSize.BytesInMegaByte);
            imageApis.MapGet("get/{uuid}", (string uuid) =>
            {
                using var db = new BinariesDbContext();
                var res = db.Binaries.FirstOrDefault(i => i.Uuid == uuid); 
                if(res is null)
                {
                    return Results.NotFound(); 
                }
                return Results.File(res.Content, contentType: res.MimeType, fileDownloadName: res.FileName); 
            }); 
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
                using var imagesDb = new BinariesDbContext(); 
                return Results.Ok(new
                {
                    UserCount = db.Users.Count(),
                    BoardCount = db.Boards.Count(),
                    PostCount = db.Posts.Count(),
                    MessageCount = db.Messages.Count(),
                    ImageCount = imagesDb.Binaries.Count(),
                });
            }); 
            return app; 
        }
    }
}