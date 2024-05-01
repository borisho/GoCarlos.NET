using CommunityToolkit.Mvvm.Messaging.Messages;

namespace GoCarlos.NET.Messages;

public class BoolMessage(bool value) : ValueChangedMessage<bool>(value)
{
}
