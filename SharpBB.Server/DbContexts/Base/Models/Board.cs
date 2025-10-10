using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace SharpBB.Server.DbContexts.Base.Models;

[PrimaryKey(nameof(Uuid))]
[Index(nameof(OwnerUuid), nameof(BelongGroupUuid))]
public class Board
{
    [MaxLength(36)]
    public required string Uuid { get; set; }
    [MaxLength(255)]
    public required string Title { get; set; }
    [MaxLength(1000)]
    public string? Description { get; set; }
    public byte[]? Icon { get; set; }
    public required DateTime Created { get; set;  }
    /// <summary>
    /// Ready-to-serialize string; will be a list, fulfilled with <see cref="BoardPointSystemData"/>, null for none. 
    /// </summary>
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    private string? PointDataInternal { get; set;  }
    
    [NotMapped]
    public BoardPointSystemData? PointData {
        get => PointDataInternal is null ? null : JsonSerializer.Deserialize<BoardPointSystemData>(PointDataInternal);
        set => PointDataInternal = value is null ? null : JsonSerializer.Serialize(value);
    }


    /// <remarks>If not set to Everyone, AllowedUsers and BoardUserPostAllowed will affect;
    /// When PointSpecified is chosen, if there's no <seealso cref="BoardAllowedPointRange"/>'s set,
    /// nobody will be allowed to post
    /// except for the explicitly allowed ones.<br/><br/>
    /// Bans will be banned whatever chosen. <br/><br/>
    /// //TODO and please design it this way. 
    /// </remarks>
    
    public enum BoardPostPermissionLevel
    {
        Everyone, 
        AllowedUsers, 
        Admin, 
        PointSpecified
    }
    public required BoardPostPermissionLevel PermissionLevel { get; set; }
    /// <summary>
    /// //TODO When ALLOW_CREATING_BOARDS conf is disabled, the owner will always be Admin. 
    /// </summary>
    [MaxLength(36)]
    public required string OwnerUuid { get; set;  }
    public User? Owner { get; set; }

    public ICollection<BoardAllowedPointRange> BoardAllowedPointRanges { get; set; } = new List<BoardAllowedPointRange>(); 
    public ICollection<BoardUserPostAllowed> BoardUserPostAllowed { get; set; } = new List<BoardUserPostAllowed>();
    public ICollection<BoardUserPostBanned> BoardUserPostBanned { get; set; } = new List<BoardUserPostBanned>();
    public ICollection<UserPoint> Points { get; set; } = new List<UserPoint>();
    public ICollection<Post> Posts { get; set; } = new List<Post>();
    public required string BelongGroupUuid { get; set;  }
    public BoardGroup? BelongGroup { get; set;  }

}