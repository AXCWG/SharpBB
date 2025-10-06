using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SharpBB.Server.DbContexts.Base;

[PrimaryKey(nameof(Uuid))]
[Index(nameof(Parent), nameof(Board))]
public class Post
{
    [MaxLength(36)]
    public required string Uuid { get; set; }
    [MaxLength(255)]
    public required string Title { get; set; }
    
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public required string Content { get; set; }
    [MaxLength(36)]
    public string? Parent { get; set; }
    [MaxLength(36)]
    public string? Board { get; set; }
}

[PrimaryKey(nameof(Uuid))]
public class Board
{
    [MaxLength(36)]
    public required string Uuid { get; set; }
    [MaxLength(255)]
    public required string Title { get; set; }
    [MaxLength(1000)]
    public string? Description { get; set; }
    /// <summary>
    /// Ready-to-serialize string; will be a list, fulfilled with <see cref="BoardPointSystemData"/>
    /// </summary>
    public required string Data { get; set;  }
}

[PrimaryKey(nameof(Uuid))]
[Index(nameof(Username), IsUnique = true)]
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
    public required UserRole Role { get; set;  }
    
}

[PrimaryKey(nameof(Uuid))]
[Index(nameof(UserUuid))]
[Index(nameof(Board))]
public class Point
{
    [MaxLength(36)]
    public required string Uuid { get; set;  }
    [MaxLength(36)]
    public required string UserUuid { get; set;  }
    [MaxLength(36)]
    public required string Board { get; set;  }
    public required int Level { get; set;  }
}

/// <summary>
/// Should not be used for Entity Framework directly. 
/// </summary>
public class BoardPointSystemData
{
    /// <summary>
    /// Included.
    /// </summary>
    public required ulong Left { get; set;  }
    /// <summary>
    /// Not included. 
    /// </summary>
    public required ulong Right { get; set;  }
    public required string Tag { get; set;  }
}

[PrimaryKey(nameof(Uuid))]
public class Announce
{
    public required string Uuid { get; set;  }
    public required string Content { get; set;  }
}

[PrimaryKey(nameof(Uuid))]
[Index(nameof(From))]
[Index(nameof(To))]
public class Message
{
    [MaxLength(36)]
    public required string Uuid { get; set;  }
    [MaxLength(10000)]
    public required string Content { get; set;  }
    [MaxLength(36)]
    public required string From { get; set;  }
    [MaxLength(36)]
    public required string To { get; set;  }
    
}

[PrimaryKey(nameof(Uuid))]
public class Image
{
    [MaxLength(36)]
    public required string Uuid { get; set;  }
    public required byte[] Content { get; set;  }
}
public enum UserRole
{
    Admin, Banned, People
}