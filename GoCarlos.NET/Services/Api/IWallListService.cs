using System.Data;

namespace GoCarlos.NET.Services.Api;

/// <summary>
/// Service for managing DataTable for WallList in main window
/// </summary>
public interface IWallListService
{
    /// <summary>
    /// Create empty DataTable with predefined columns
    /// </summary>
    DataTable Create();
}
