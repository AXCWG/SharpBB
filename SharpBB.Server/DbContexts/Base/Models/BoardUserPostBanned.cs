using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SharpBB.Server.DbContexts.Base.Models;

[Index(nameof(UserUuid), nameof(BoardUuid))]
[PrimaryKey(nameof(Uuid))]
public class BoardUserPostBanned
{
    [MaxLength(36)]
    public required string Uuid { get; set; }
    [MaxLength(36)]
    public required string UserUuid { get; set;  }
    public required User User { get; set; }
    [MaxLength(36)]
    public required string BoardUuid { get; set;  }
    public required Board Board { get; set; }
    public required DateTime CreatedOn { get; set; }
    /// <summary>
    /// Null for perma. 
    /// </summary>
    public TimeSpan? For { get; set; }
}