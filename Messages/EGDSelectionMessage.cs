using CommunityToolkit.Mvvm.Messaging.Messages;
using GoCarlos.NET.Models;

namespace GoCarlos.NET.Messages;

public class EGDSelectionMessage(EGD_Data value) : ValueChangedMessage<EGD_Data>(value)
{
}
