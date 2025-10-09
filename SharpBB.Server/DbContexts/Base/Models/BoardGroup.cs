using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharpBB.Server.DbContexts.Base.Models
{
    [PrimaryKey(nameof(Uuid))]
    [Index(nameof(ParentUuid))]
    public class BoardGroup
    {
        [MaxLength(36)]
        public required string Uuid { get; set; }
        [MaxLength(255)]
        public required string Title { get; set;  }
        public string? Description { get; set;  }
        public ICollection<Board> Boards { get; set; } = new List<Board>();

        /// <summary>
        /// Can be nested. 
        /// </summary>
        public string? ParentUuid { get; set;  }
        public BoardGroup? Parent { get; set;  }
        public ICollection<BoardGroup> BoardGroups { get; set; } = new List<BoardGroup>(); 
        
    }
}
