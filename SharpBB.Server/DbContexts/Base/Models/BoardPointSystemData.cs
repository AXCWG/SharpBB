namespace SharpBB.Server.DbContexts.Base.Models;

/// <summary>
/// Should not be used for Entity Framework directly. <seealso cref="Board"/>
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