using CommunityToolkit.Mvvm.Messaging;

namespace GoCarlos.NET.Interfaces;

/// <summary>
/// An interface marking object that has to clean <see cref="WeakReferenceMessenger" /> or <see cref="StrongReferenceMessenger" />
/// </summary>
public interface IReferenceCleanup
{
    /// <summary>
    /// Unregisters a recipient from messages of a given type by calling <see cref="WeakReferenceMessenger.Unregister{TMessage, TToken}(object, TToken)" />.
    /// </summary>
    void Unregister();
}
