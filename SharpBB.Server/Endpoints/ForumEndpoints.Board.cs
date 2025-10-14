using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharpBB.Server.DbContexts;
using SharpBB.Server.DbContexts.Base.Models;
using SharpBB.Server.DbContexts.Base.Models.DTOs;

namespace SharpBB.Server.Endpoints;

public static partial class ForumEndpoints
{
    extension(WebApplication app)
    {
        public WebApplication MapBbsBoardEndpoints()
        {
            var boardApis = app.MapGroup("/api/bbs/board");
            boardApis.MapGet("get", (HttpContext context, int? boardGroupId) =>
            {
                using var db = INTERN_CONF_SINGLETONS.MainContext;
                IOrderedQueryable<Board> res;
                if ((!boardGroupId.HasValue))
                {
                    res = db.Boards.Include(e => e.Posts.OrderByDescending(i => i.DateTime).Take(1)).OrderByDescending(i => i.Title);
                    return Results.Ok(res.Select(i => (GetBoardPayload)i).ToList());
                }

                res = db.Boards.Where(i => i.BelongGroupId == boardGroupId).Include(e => e.Posts.OrderByDescending(i => i.DateTime).Take(1)).OrderByDescending(o => o.Title);
                return Results.Ok(res.Select(i => (GetBoardPayload)i).ToList());

            });
            boardApis.MapPost("create", (HttpContext context, [FromBody] CreateBoardBody body) =>
            {
                var sessionUuid = context.Session.GetString("uuid");
                using var conf = new ConfigurationSqliteDbContext();
                using var db = INTERN_CONF_SINGLETONS.MainContext;
                if (!conf.Settings.AllowUserCreatingBoards &&
                    db.Users.First(i => i.Uuid == sessionUuid).Role != User.UserRole.Admin)
                {
                    return Results.BadRequest();
                }
                try
                {
                    db.Boards.Add(new()
                    {
                        BelongGroupId = body.Under,
                        Uuid = Guid.NewGuid().ToString(),
                        Title = body.Title,
                        Description = body.Description,
                        PermissionLevel = Board.BoardPostPermissionLevel.Everyone,
                        Created = DateTime.UtcNow,
                        OwnerUuid = sessionUuid ?? throw new NullReferenceException()
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
    }

    private class CreateBoardBody
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        // TODO Needs implement. 
        public required int Under { get; set; }

    }
}