namespace SharpBB.Server.DbContexts.Base.Models.DTOs;

public class GetBoardPayload
{
    public required string Uuid { get; set;  }
    public string? Name { get; set;  }
    public string? Description { get; set;  }
    public DateTime? DateCreated { get; set;  }
    public GetPostPayload? LastestActivity { get; set;  }

    public static implicit operator GetBoardPayload(Board board)
    {
        return new GetBoardPayload()
        {
            Uuid=board.Uuid, 
            Name = board.Title, DateCreated = board.Created, Description = board.Description, LastestActivity = board.Posts.Select(i=>new GetPostPayload
            {
                Uuid = i.Uuid,
                Content = i.Content,
                DateTime = i.DateTime,
                Title = i.Title,
                ChildrenUuid = [],
            }).FirstOrDefault() ?? null
        }; 
    }
}