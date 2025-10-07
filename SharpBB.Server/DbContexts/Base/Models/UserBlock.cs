using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SharpBB.Server.DbContexts.Base.Models;

/// <summary>
/// Message sending will fail; Post and replies will be invisible
/// </summary>
[Index(nameof(ToUuid), nameof(ByUuid))]
[PrimaryKey(nameof(Uuid))]
public class UserBlock
{
    [MaxLength(36)]
    public required string Uuid { get; set; }
    [MaxLength(36)]
    public required string ByUuid { get; set; }
    public required User By { get; set; }
    
    [MaxLength(36)]
    public required string ToUuid { get; set; }
    public required User To { get; set; }
}