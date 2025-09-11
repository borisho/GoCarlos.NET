using Microsoft.Extensions.Localization;

namespace GoCarlos.NET.Services.Api;

/// <summary>
/// Provides localization
/// </summary>
public interface ILocalizerService
{
    LocalizedString this[string name] { get; }
}
