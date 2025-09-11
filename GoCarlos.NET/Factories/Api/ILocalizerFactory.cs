using GoCarlos.NET.Enums;
using GoCarlos.NET.Services.Api;

namespace GoCarlos.NET.Factories.Api;

public interface ILocalizerFactory
{
    ILocalizerService GetByQualifier(LocalizerType type);
}
