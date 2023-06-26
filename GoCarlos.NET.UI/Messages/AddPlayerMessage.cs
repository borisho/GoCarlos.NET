using GoCarlos.NET.UI.Models;

namespace GoCarlos.NET.UI.Messages;

public class AddPlayerMessage
{
    public AddPlayerMessage(Player player, bool param)
    {
        Player = player;
        Param = param;
    }

    public Player Player { get; }
    public bool Param { get; }
}
