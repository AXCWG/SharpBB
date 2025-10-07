using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SharpBB.Server.DbContexts.Base.Models;
[PrimaryKey(nameof(Uuid))]
[Index(nameof(UserUuid), nameof(BoardUuid))]
public class UserPoint
{
    [MaxLength(36)]
    public required string Uuid { get; set; }
    [MaxLength(36)]
    public required string UserUuid { get; set;  }
    public required User User { get; set; }
    [MaxLength(36)]
    public required string BoardUuid { get; set;  }
    public required Board Board { get; set; }
    public required int Level { get; set;  }
}