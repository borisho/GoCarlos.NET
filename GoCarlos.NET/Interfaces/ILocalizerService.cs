using Microsoft.Extensions.Localization;

namespace GoCarlos.NET.Interfaces;

/// <summary>
/// Provides localization
/// </summary>
public interface ILocalizerService
{
    LocalizedString this[string name] { get; }
}
