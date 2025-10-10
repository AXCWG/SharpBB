using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace SharpBB.Server.DbContexts.Base.Models;

[PrimaryKey(nameof(Uuid))]
[Index(nameof(ParentUuid), nameof(BoardUuid), nameof(ByUuid), nameof(TopParentUuid))]
public class Post
{
    [MaxLength(36)]
    public required string Uuid { get; set; }
    /// <summary>
    /// Nullable cuz will not be there if it's below a post. 
    /// </summary>
    [MaxLength(255)]
    public string? Title { get; set; }
    
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public required string Content { get; set; }
    /// <summary>
    /// Null if OP post. 
    /// </summary>
    [MaxLength(36)]
    public string? ParentUuid { get; set; }
    public Post? Parent { get; set; } 
    public ICollection<Post>  Children { get; set; } = new List<Post>();
    
    [MaxLength(36)]
    public string? TopParentUuid { get; set; }
    public Post? TopParent { get; set; }
    public ICollection<Post> AllChildren { get; set; } = new List<Post>(); 
    public required DateTime DateTime { get; set;  }

    [MaxLength(36)]
    public required string BoardUuid { get; set; }
    public Board? Board { get; set; }
    [MaxLength(36)]
    public string? ByUuid { get; set; }
    public User? By { get; set; }
    public ICollection<PostAllowedPointRange> AllowedPointRanges { get; set; } = new List<PostAllowedPointRange>();
    public ICollection<PostUserPostAllowed>  UserPostAllowed { get; set; } = new List<PostUserPostAllowed>();
    public ICollection<PostUserPostBanned>  UserPostBanned { get; set; } = new List<PostUserPostBanned>();
}