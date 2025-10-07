using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SharpBB.Server.DbContexts.Base.Models;

/// <summary>
/// <c>LeftBound</c> and <c>RightBound</c>: set to null for "infinity". 
/// </summary>
[PrimaryKey(nameof(Uuid))]
[Index(nameof(LeftBound),nameof(PostUuid), nameof(RightBound))]
public class PostAllowedPointRange
{
    [MaxLength(36)]
    public required string Uuid { get; set; }
    [MaxLength(36)]
    public required string PostUuid { get; set;  }
    public required Post Post{get;set;}
    public int? LeftBound { get; set; }
    public int? RightBound { get; set; }
}