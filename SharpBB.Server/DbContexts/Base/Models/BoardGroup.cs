using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SharpBB.Server.DbContexts.Base.Models
{
    [PrimaryKey(nameof(Id))]
    
    public class BoardGroup
    {
    
        
        public int Id { get; set;  }
        [MaxLength(255)]
        public required string Title { get; set;  }
        public string? Description { get; set;  }
        public ICollection<Board> Boards { get; set; } = new List<Board>();
        
        
    }
}
