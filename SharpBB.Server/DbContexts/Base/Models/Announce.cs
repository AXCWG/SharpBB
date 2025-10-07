using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SharpBB.Server.DbContexts.Base.Models;

[PrimaryKey(nameof(Uuid))]
public class Announce
{
    [MaxLength(36)]
    public required string Uuid { get; set;  }
    public required string Content { get; set;  }
    /// <summary>
    /// Null for sticky/pinned. 
    /// </summary>
    public DateTime? Timestamp { get; set;  }
}