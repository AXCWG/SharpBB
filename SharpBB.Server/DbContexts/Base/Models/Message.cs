using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SharpBB.Server.DbContexts.Base.Models;

[PrimaryKey(nameof(Uuid))]
[Index(nameof(FromUuid), nameof(ToUuid))]
public class Message
{
    [MaxLength(36)]
    public required string Uuid { get; set;  }
    [MaxLength(10000)]
    public required string Content { get; set;  }
    [MaxLength(36)]
    public required string FromUuid { get; set;  }
    public required User From { get; set; }
    [MaxLength(36)]
    public required string ToUuid { get; set;  }
    public required User To { get; set; }
    
}