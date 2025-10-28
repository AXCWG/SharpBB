using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharpBB.Server.DbContexts.Base.Models;
using SharpBB.Server.DbContexts.Base.Models.DTOs;

namespace SharpBB.Server.Endpoints;

public static partial class ForumEndpoints
{
    extension(WebApplication app)
    {
        public WebApplication MapBbsBoardGroupEndpoints()
        {
            var boardGroupApis = app.MapGroup("/api/bbs/boardgroup");
            boardGroupApis.MapPost("create", (HttpContext context, [FromBody] CreateBoardGroupBody body) =>
            {
                using var db = INTERN_CONF_SINGLETONS.MainContext;
                var sessionUuid = context.Session.GetString("uuid");
                if (db.Users.First(i => i.Uuid == sessionUuid).Username != "Admin")
                {
                    return Results.Forbid();
                }

                try
                {
                    db.BoardGroups.Add(new BoardGroup
                    {
                        Title = body.Title,
                        Description = body.Description
                    });
                    db.SaveChanges();
                    return Results.Ok();
                }
                catch (Exception e)
                {
                    return GeneralHandler(e);
                }

            });
            boardGroupApis.MapGet("get", (HttpContext context) =>
            {
                using var db = INTERN_CONF_SINGLETONS.MainContext;
                var results = db.BoardGroups.Include(i => i.Boards.OrderBy(i => i.Title)).ThenInclude(i=>i.Posts).OrderBy(i => i.Id).Select(i => (GetBoardGroupPayload)i).ToList();
                return results;
            });
            return app;
        }
    }

    private class CreateBoardGroupBody
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
    }
}