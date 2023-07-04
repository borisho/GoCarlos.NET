namespace GoCarlos.NET.Interfaces;

/// <summary>
/// An interface defining object that can be closed
/// </summary>
public interface ICloseable
{
    /// <summary>
    /// Closes the <see cref="ICloseable" /> object
    /// </summary>
    void Close();
}
