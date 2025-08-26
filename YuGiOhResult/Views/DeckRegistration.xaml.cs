using CommunityToolkit.Maui.Behaviors;
using YuGiOhResult.ViewModels;

namespace YuGiOhResult.Views;

public partial class DeckRegistration : ContentPage
{
	public DeckRegistration(DeckRegistrationViewModel viewModel)
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