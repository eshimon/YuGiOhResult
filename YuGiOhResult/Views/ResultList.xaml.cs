using CommunityToolkit.Maui.Behaviors;

namespace YuGiOhResult.Views;

public partial class ResultList : ContentPage
{
	public ResultList()
	{
		InitializeComponent();

        // BindingContext‚ğİ’è
        var viewModel = new ViewModels.ResultListViewModel();
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