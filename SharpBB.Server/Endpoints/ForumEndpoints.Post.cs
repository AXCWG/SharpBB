using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharpBB.Server.DbContexts;
using SharpBB.Server.DbContexts.Base.Models.DTOs;

namespace SharpBB.Server.Endpoints;

public static partial class ForumEndpoints
{
    extension(WebApplication app)
    {
        public WebApplication MapBbsPostEndpoints()
        {
            // TODO rethink endpoints. 
            // ReSharper disable once InvalidXmlDocComment
            /**
             * Directory-like structure get?
             *
             */
            // TODO need test. 
            var postApi = app.MapGroup("/api/bbs/post");
            
            postApi.MapGet("get/{*parameters}", (HttpContext context, string? parameters, bool? flat) =>
            {
                using var db = INTERN_CONF_SINGLETONS.MainContext;

                if (flat ?? true)
                {
                    return Results.Ok(db.Posts.OrderBy(i => i.DateTime).Select(i=>(GetPostPayload)i).ToList()); 
                }
                var ses = parameters?.Trim().Split("/").ToList() ?? [];
                for (int i = 0; i < ses.Count; i++)
                {
                    if (ses[i].IsNullOrWhiteSpace())
                    {
                        return Results.NotFound(); 
                    }
                }
                for (var index = 0; index < ses.Count; index++)
                {
                    var indexInner = index;
                    var a = db.Posts.Where(i =>

                         (i.Uuid == ses[indexInner]) && (i.ParentUuid == (indexInner == 0 ? null : ses[indexInner - 1]))
                    );
                    if ( !a.Any())
                    {
                        
                        return Results.NotFound();
                    }
                }
                return Results.Ok(db.Posts.Include(i => i.Children.OrderBy(inner => inner.DateTime)).Where(i => i.ParentUuid == ses.LastOrDefault()).Select(i => (GetPostPayload)i).ToList());

             
            }).WithDescription("Do not add trailing slash.");
            postApi.MapPost("post/{*parameters}", (HttpContext context, string? parameters, [FromBody] PostPostBody body) =>
            {
                using var db = INTERN_CONF_SINGLETONS.MainContext;
                using var conf = new ConfigurationSqliteDbContext();
                var sessionUuid = context.Session.GetString("uuid");

                if (!conf.Settings.AllowAnonymousPost && sessionUuid.IsNullOrWhiteSpace())
                {
                    return Results.BadRequest();
                }

                var ses = parameters?.Trim().Split('/').ToList() ?? [];
                for (int i = 0; i < ses.Count; i++)
                {
                    if (ses[i].IsNullOrWhiteSpace())
                    {
                        return Results.NotFound();
                    }
                }
                for (var index = 0; index < ses.Count; index++)
                {
                    var indexInner = index;
                    var a = db.Posts.Where(i =>

                         (i.Uuid == ses[indexInner]) && (i.ParentUuid == (indexInner == 0 ? null : ses[indexInner - 1]))
                    );
                    if (!a.Any())
                    {

                        return Results.NotFound();
                    }
                }

                db.Posts.Add(new()
                {
                    Uuid = Guid.NewGuid().ToString(),
                    Title = body.Title,
                    Content = body.Content,
                    DateTime = DateTime.UtcNow,
                    BoardUuid = ses.Count == 0 ? body.Under ?? throw new BadHttpRequestException("Required field: 'Under' while posting post at root level. ") : db.Posts.First(i => i.Uuid == ses.First()).BoardUuid,
                    ByUuid = sessionUuid,
                    ParentUuid = ses.LastOrDefault(),
                    TopParentUuid = ses.FirstOrDefault()
                });
                db.SaveChanges();


                return Results.Ok();


            }).WithDescription("Do not add trailing slash.");
            return app;
        }
    }

    private class PostPostBody
    {
        public required string Title { get; set; }
        public required string Content { get; set; }
        /// <summary>
        /// Board uuid when creating a brand-new post. 
        /// </summary>
        public string? Under { get; set; }
    }
    private enum GetPostOrderType
    {
        DateTime, Alphabetical
    }
}