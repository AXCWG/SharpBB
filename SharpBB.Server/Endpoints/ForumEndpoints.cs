using ByteSizeLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharpBB.Server.DbContexts;
using SharpBB.Server.DbContexts.Base.Models;
using SharpBB.Server.DbContexts.Base.Models.DTOs;
using WebPWrapper.Encoder;

namespace SharpBB.Server.Endpoints;

public static partial class ForumEndpoints
{
    private static IResult GeneralHandler(Exception e)
    {
        using var conf = new ConfigurationSqliteDbContext();
        return Results.InternalServerError(new { Message = e.InnerException is null ? e.Message : e.InnerException.Message, conf.Settings.AdminContact });
    }
    extension(WebApplication app)
    {
        public WebApplication MapBbsEndpoints()
        {
            app.MapBbsUserEndpoints().MapBbsPostEndpoints().MapBbsStatusEndpoints().MapBbsImageEndpoints()
                .MapBbsBoardEndpoints().MapBbsBoardGroupEndpoints();
            return app;
        }
    }
}