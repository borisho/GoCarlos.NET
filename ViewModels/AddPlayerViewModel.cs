namespace GoCarlos.NET.ViewModels;

public partial class AddPlayerViewModel : PlayerControlViewModel
{
    public AddPlayerViewModel(string title, int rounds) : base(rounds)
    {
        Title = title;
    }
}
