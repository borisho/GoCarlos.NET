using CommunityToolkit.Mvvm.Messaging.Messages;
using GoCarlos.NET.UI.Models;

namespace GoCarlos.NET.UI.Messages;

public class EgdMessage : ValueChangedMessage<EgdData>
{
    public EgdMessage(EgdData value) : base(value)
    {
    }
}
