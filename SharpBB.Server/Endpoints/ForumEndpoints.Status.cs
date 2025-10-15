using SharpBB.Server.DbContexts;

namespace SharpBB.Server.Endpoints;

public static partial class ForumEndpoints
{
    private enum MetadataRequestType
    {
        Name,
        Description
    }
    extension(WebApplication app)
    {
        public WebApplication MapBbsStatusEndpoints()
        {
            app.MapGet("/api/bbs/conf/ico", () =>
            {
                using var conf = new ConfigurationSqliteDbContext();
                if (conf.Settings.ForumIcon is null)
                {
                    return Results.NotFound(); 
                }
                return Results.File(conf.Settings.ForumIcon, "image/webp", fileDownloadName: "ico.webp");
            }); 
            app.MapGet("/api/bbs/conf", () =>
            {
                using var conf = new ConfigurationSqliteDbContext();
                return Results.Ok(new
                {
                    conf.Settings.ForumName, 
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
                    conf.Settings.EnforceEmail, conf.Settings.EnableLoginWithEmail
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
            app.MapGet("/api/bbs/metadata", (MetadataRequestType? type) =>
            {
                using var conf = new ConfigurationSqliteDbContext();
                switch (type)
                {
                    case MetadataRequestType.Name:
                        return Results.Ok(conf.Settings.ForumName);
                    case MetadataRequestType.Description:
                        return Results.Ok(conf.Settings.ForumDescription);
                    default:
                        return Results.BadRequest();
                }
            });
            return app;
        }
    }

   
}