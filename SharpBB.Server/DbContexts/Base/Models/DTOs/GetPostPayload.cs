using System.Diagnostics.CodeAnalysis;

namespace SharpBB.Server.DbContexts.Base.Models.DTOs;

public class GetPostPayload
{
    public required string Uuid { get; set;  }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public DateTime? DateTime { get; set;  }
    public required string[] ChildrenUuid { get; set; }
    

    
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
            ChildrenUuid = post.AllChildren.OrderByDescending(i=>i.DateTime).Select(i=>i.Uuid).ToArray()
        }; 
        return payload;
    }
}

