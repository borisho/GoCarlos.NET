using CommunityToolkit.Mvvm.Messaging.Messages;

namespace GoCarlos.NET.Messages;

public class BoolMessage : ValueChangedMessage<bool>
{
    public BoolMessage(bool value) : base(value) { }
}
