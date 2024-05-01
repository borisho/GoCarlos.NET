using CommunityToolkit.Mvvm.Messaging.Messages;
using GoCarlos.NET.Models;

namespace GoCarlos.NET.Messages;

public class AddPlayerMessage(Player value) : ValueChangedMessage<Player>(value)
{
}
