using GoCarlos.NET.Enums;
using Microsoft.Extensions.Localization;

namespace GoCarlos.NET.Interfaces;

public interface IWindowService
{
    LocalizedString this[string name] { get; }
    void Show(Windows type);
}
