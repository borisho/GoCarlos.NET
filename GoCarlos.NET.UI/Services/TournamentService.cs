using CommunityToolkit.Mvvm.Messaging;
using GoCarlos.NET.Core.Interfaces;
using GoCarlos.NET.UI.Interfaces;
using GoCarlos.NET.UI.Messages;

namespace GoCarlos.NET.UI.Services;

public sealed class TournamentService : ITournamentService, IRecipient<AddPlayerMessage>
{
    public TournamentService(ITournament tournament)
    {
        Tournament = tournament;

        WeakReferenceMessenger.Default.Register(this);
    }

    public ITournament Tournament { get; set; }

    public void Receive(AddPlayerMessage message)
    {
        /*Tournament.Add(message.Player);

        if (message.Param)
        {
            _windowService.ShowAddPlayerWindowWithParam(message.Param);
        }*/
    }
}
