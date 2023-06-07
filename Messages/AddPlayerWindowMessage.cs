using CommunityToolkit.Mvvm.Messaging.Messages;
using GoCarlos.NET.Models;

namespace GoCarlos.NET.Messages;

public class AddPlayerWindowMessage : ValueChangedMessage<Player>
{
    public AddPlayerWindowMessage(Player value) : base(value)
    {
    }
}
