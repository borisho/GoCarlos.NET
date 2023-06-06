using CommunityToolkit.Mvvm.Messaging.Messages;
using GoCarlos.NET.Models;

namespace GoCarlos.NET.Messages;

public class AddPlayerMessage : ValueChangedMessage<Player>
{
    public AddPlayerMessage(Player value) : base(value)
    {
    }
}
