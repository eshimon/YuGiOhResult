using CommunityToolkit.Maui.Behaviors;

namespace YuGiOhResult.Views;

public partial class DeckRegistration : ContentPage
{
	public DeckRegistration()
	{
		InitializeComponent();

        // BindingContext‚ğİ’è
        var viewModel = new ViewModels.DeckRegistrationViewModel();
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