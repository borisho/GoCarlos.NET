using CommunityToolkit.Mvvm.Messaging.Messages;
using GoCarlos.NET.Models;

namespace GoCarlos.NET.Messages;

public class EgdMessage : ValueChangedMessage<EgdData>
{
    public EgdMessage(EgdData value) : base(value)
    {
    }
}
