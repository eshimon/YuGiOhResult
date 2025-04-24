namespace YuGiOhResult.Views;
using CommunityToolkit.Maui.Behaviors;

public partial class DeckList : ContentPage
{
	public DeckList()
	{
		InitializeComponent();

        // BindingContext‚ğİ’è
        var viewModel = new ViewModels.DeckListViewModel();
        BindingContext = viewModel;

        // EventToCommandBehavior‚ğContentPage‚ÌBehaviors‚É’Ç‰Á
        var behavior = new EventToCommandBehavior
        {
            EventName = nameof(Appearing),
            Command = viewModel.AppearingCommand,
        };
        Behaviors.Add(behavior);
    }
}