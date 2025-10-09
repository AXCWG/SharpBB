namespace SharpBB.Server.DbContexts.Base.Models.DTOs;

public class GetPostPayload
{
    public required string Uuid { get; set;  }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public DateTime? DateTime { get; set;  }
    public List<GetPostPayload>? Children { get; set; }
    public string? BoardUuid { get; set; }

    public static implicit operator GetPostPayload(Post post)
    {
        var payload = new GetPostPayload()
        {
            Uuid = post.Uuid,
            Title = post.Title,
            Content = post.Content,
            DateTime = post.DateTime,
            BoardUuid = post.BoardUuid,
            Children = post.Children.Select(i=>(GetPostPayload)i).ToList()
        }; 
        return payload;
    }
    
}