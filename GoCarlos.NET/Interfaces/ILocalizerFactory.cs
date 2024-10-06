using GoCarlos.NET.Enums;

namespace GoCarlos.NET.Interfaces;

public interface ILocalizerFactory
{
    ILocalizerService GetByQualifier(LocalizerType type);
}
