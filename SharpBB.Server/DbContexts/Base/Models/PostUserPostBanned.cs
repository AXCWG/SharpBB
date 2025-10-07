using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SharpBB.Server.DbContexts.Base.Models;

[PrimaryKey(nameof(Uuid))]
[Index(nameof(UserUuid), nameof(PostUuid))]

public class PostUserPostBanned
{
    [MaxLength(36)]
    public required string Uuid { get; set; }
    [MaxLength(36)]
    public required string UserUuid { get; set;  }
    public required User User { get; set; }
    
    [MaxLength(36)]
    public required string PostUuid { get; set;  }
    public required Post Post { get; set; }
    public required DateTime CreatedOn { get; set; }
    /// <summary>
    /// Null for perma. 
    /// </summary>
    public TimeSpan? For { get; set; }
}