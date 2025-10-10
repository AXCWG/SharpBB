namespace SharpBB.Server.DbContexts.Base.Models.DTOs
{
    public class GetBoardGroupPayload
    {
        public required int Id { get; set;  }
        public string? Title { get; set;  }
        public string? Description { get; set;  }
        public List<GetBoardPayload> Boards { get; set; } = new(); 
        public static implicit operator GetBoardGroupPayload(BoardGroup group)
        {
            var res = new GetBoardGroupPayload()
            {
                Boards = group.Boards.Select(i => (GetBoardPayload)i).ToList(),
                Id = group.Id,
                Title = group.Title,
                Description = group.Description,
            };
            return res; 
        }

    }
}
