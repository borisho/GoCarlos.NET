using CommunityToolkit.Mvvm.Messaging.Messages;
using GoCarlos.NET.Models;

namespace GoCarlos.NET.Messages;

public class EGDSelectionMessage : ValueChangedMessage<EGD_Data>
{
    public EGDSelectionMessage(EGD_Data value) : base(value)
    {
    }
}
