using CommunityToolkit.Maui.Behaviors;
using YuGiOhResult.ViewModels;

namespace YuGiOhResult.Views
{
    public partial class MainPage : ContentPage
    {

        public MainPage(MainPageViewModel viewModel)
        {

            InitializeComponent();

            // BindingContextを設定
            BindingContext = viewModel;

            // EventToCommandBehaviorをContentPageのBehaviorsに追加
            var behavior = new EventToCommandBehavior
            {
                EventName = nameof(Appearing),
                Command = viewModel.AppearingCommand,
            };
            Behaviors.Add(behavior);
        }

    }

}
