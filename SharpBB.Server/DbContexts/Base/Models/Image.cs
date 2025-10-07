using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SharpBB.Server.DbContexts.Base.Models;

[PrimaryKey(nameof(Uuid))]
public class Image
{
    [MaxLength(36)]
    public required string Uuid { get; set;  }
    public required byte[] Content { get; set;  }
}