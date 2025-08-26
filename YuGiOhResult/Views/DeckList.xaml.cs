namespace YuGiOhResult.Views;
using CommunityToolkit.Maui.Behaviors;
using YuGiOhResult.ViewModels;

public partial class DeckList : ContentPage
{
	public DeckList(DeckListViewModel viewModel)
	{
		InitializeComponent();

        // BindingContext‚ğİ’è
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