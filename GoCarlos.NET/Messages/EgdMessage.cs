using CommunityToolkit.Mvvm.Messaging.Messages;
using GoCarlos.NET.Models;

namespace GoCarlos.NET.Messages;

public class EgdMessage(EgdData value) : ValueChangedMessage<EgdData>(value)
{
}
