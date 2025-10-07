using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SharpBB.Server.DbContexts.Base.Models;

[Index(nameof(UserUuid), nameof(BoardUuid))]
[PrimaryKey(nameof(Uuid))]
public class BoardUserPostAllowed
{
    [MaxLength(36)]
    public required string Uuid { get; set; }
    [MaxLength(36)]
    public required string UserUuid { get; set;  }
    public required User User { get; set; }
    [MaxLength(36)]
    public required string BoardUuid { get; set;  }
    public required Board Board { get; set; }
}