namespace SharpBB.Server.DbContexts.Base.Models.DTOs;

public class GetBoardPayload
{
    public required string Uuid { get; set;  }
    public string? Name { get; set;  }
    public string? Description { get; set;  }
    public DateTime? DateCreated { get; set;  }
    public GetPostPayload? LatestActivity { get; set;  }
    public int TopicCount { get; set; }
    public int RepliesCount { get; set; }
    public static implicit operator GetBoardPayload(Board board)
    {
        return new GetBoardPayload()
        {
            Uuid=board.Uuid, 
            Name = board.Title, DateCreated = board.Created, Description = board.Description, LatestActivity = board.Posts.OrderByDescending(i=>i.DateTime).Select(i=> i.TopParentUuid is null ? new GetPostPayload
            {
                Uuid = i.Uuid,
                Content = null,
                DateTime = i.DateTime,
                Title = i.Title,
                ChildrenUuids = [],
                ParentUuid = i.ParentUuid, By = i.ByUuid, 
            } : new()
            {
                Uuid = i.TopParent!.Uuid, ParentUuid = i.TopParent.ParentUuid, ChildrenUuids = [], Title = i.TopParent.Title, DateTime = i.DateTime, Content = null, By = i.TopParent.ByUuid
            }).FirstOrDefault() ?? null, TopicCount = board.Posts.Count(i=>i.ParentUuid is null), RepliesCount = board.Posts.Count(i=>i.ParentUuid is not null)
        }; 
    }
}