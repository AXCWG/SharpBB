using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SharpBB.Server.DbContexts.Base.Models;

[PrimaryKey(nameof(Uuid))]
public class Binaries
{
    [MaxLength(36)]
    public required string Uuid { get; set;  }
    public required string FileName { get; set;  }
    public required byte[] Content { get; set;  }
    
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public required string MimeType { get; set;  }
}