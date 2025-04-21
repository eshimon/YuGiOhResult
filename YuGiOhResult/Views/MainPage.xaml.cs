using CommunityToolkit.Maui.Behaviors;

namespace YuGiOhResult.Views
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {

            InitializeComponent();

            // BindingContextを設定
            var viewModel = new ViewModels.MainPageViewModel();
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
