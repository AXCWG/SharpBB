using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SharpBB.Server.DbContexts.Base.Models;

[PrimaryKey(nameof(Uuid))]
[Index(nameof(Username), IsUnique = true)]
[Index(nameof(Email))]
public class User
{
    [MaxLength(36)]
    public required string Uuid { get; set; }
    [MaxLength(255)]
    public string? Username { get; set; }
    public byte[]? Profile { get; set; }
    [MaxLength(64)]
    public string? Password { get; set; }
    [MaxLength(255)]
    public string? Email { get; set; }
    
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? Description { get; set;  }
    public DateTime Joined { get; set;  }
    // TODO if admin, could promote others as sub-admins. Sub-admins could promote other sub-admins. 
    public enum UserRole
    {
        Admin, SubAdmin, Banned, People
    }
    public required UserRole Role { get; set;  }
    public ICollection<BoardUserPostAllowed>  BoardUserPostAllowed { get; set; } = new List<BoardUserPostAllowed>();
    public ICollection<BoardUserPostBanned> BoardUserPostBanned { get; set; } = new List<BoardUserPostBanned>();
    public ICollection<PostUserPostBanned> PostUserPostBanned { get; set; } = new List<PostUserPostBanned>();
    public ICollection<PostUserPostAllowed> PostUserPostAllowed { get; set; } = new List<PostUserPostAllowed>();
    public ICollection<UserBlock>  BlockIsBy { get; set; } = new List<UserBlock>();
    public ICollection<UserBlock>  BlockIsTo { get; set; } = new List<UserBlock>();
    public ICollection<UserPoint>  Points { get; set; } = new List<UserPoint>();
    public ICollection<Board>  Boards { get; set; } = new List<Board>();
    public ICollection<Post> Posts { get; set; } = new List<Post>();
    public ICollection<Message> Froms { get; set; } = new List<Message>();
    public ICollection<Message> Tos { get; set; } = new List<Message>();
    
}