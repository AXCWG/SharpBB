using System.Diagnostics.CodeAnalysis;

namespace SharpBB.Server.DbContexts.Base.Models.DTOs;

public class GetPostPayload
{
    public required string Uuid { get; set;  }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public DateTime? DateTime { get; set;  }
    public required string[] ChildrenUuids { get; set; }
    public required string? ParentUuid { get; set; }
    public required string? By { get; set; }
    
    public static implicit operator GetPostPayload(Post? post)
    {
        if (post is null)
        {
            return null!; 
        }
        var payload = new GetPostPayload()
        {
            Uuid = post.Uuid,
            Title = post.Title,
            Content = post.Content,
            DateTime = post.DateTime,
            ChildrenUuids = post.Children.Select(i=>i.Uuid).ToArray(), ParentUuid = post.ParentUuid, By = post.ByUuid
        }; 
        return payload;
    }
}

