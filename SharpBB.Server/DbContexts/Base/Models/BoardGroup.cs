using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SharpBB.Server.DbContexts.Base.Models
{
    [PrimaryKey(nameof(Uuid))]
    public class BoardGroup
    {
        [MaxLength(36)]
        public required string Uuid { get; set; }
        public int Order { get; set;  }
        [MaxLength(255)]
        public required string Title { get; set;  }
        public string? Description { get; set;  }
        public ICollection<Board> Boards { get; set; } = new List<Board>();
        
        
    }
}
