using CommunityToolkit.Maui.Behaviors;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration;


namespace YuGiOhResult.Views;

public partial class ResultList : ContentPage
{
    public ResultList()
    {
        InitializeComponent();

        // BindingContextを設定
        var viewModel = new ViewModels.ResultListViewModel();
        BindingContext = viewModel;

        // EventToCommandBehaviorをContentPageのBehaviorsに追加
        var behavior = new EventToCommandBehavior
        {
            EventName = nameof(Appearing),
            Command = viewModel.AppearingCommand,
        };
        Behaviors.Add(behavior);

        // Android用テンプレート
        var androidTemplate = new DataTemplate(() =>
        {

            // verticalstacklayoutを使用して、各要素を縦に並べる
            var layout = new VerticalStackLayout { Padding = 10, Spacing = 5, BackgroundColor = Colors.White };

            // Labelを作成し、逐次StackLayoutに追加
            var playedDeckNameLabel = new Label();
            playedDeckNameLabel.SetBinding(Label.TextProperty, "PlayedDeckName");
            layout.Add(playedDeckNameLabel);

            var opponentsDeckNameLabel = new Label();
            opponentsDeckNameLabel.SetBinding(Label.TextProperty, "OpponentsDeckName");
            layout.Add(opponentsDeckNameLabel);

            var coinLabel = new Label();
            coinLabel.SetBinding(Label.TextProperty, "Coin");
            layout.Add(coinLabel);

            var turnOrderLabel = new Label();
            turnOrderLabel.SetBinding(Label.TextProperty, "TurnOrder");
            layout.Add(turnOrderLabel);

            var resultLabel = new Label();
            resultLabel.SetBinding(Label.TextProperty, "Result");
            layout.Add(resultLabel);

            var memoLabel = new Label();
            memoLabel.SetBinding(Label.TextProperty, "Memo");
            layout.Add(memoLabel);

            var formattedDateTimeLabel = new Label();
            formattedDateTimeLabel.SetBinding(Label.TextProperty, "FormattedDateTime");
            layout.Add(formattedDateTimeLabel);
            
            var button = new Button
            {
                Text = "削除",
                Style = (Style)Resources["buttonStyle"]
            };
            button.SetBinding(Button.CommandProperty, new Binding("BindingContext.DeleteMatchCommand", source: this));
            button.SetBinding(Button.CommandParameterProperty, new Binding("."));
            layout.Add(button);
            return layout;
        });

        // Windows用テンプレート
        var windowsTemplate = new DataTemplate(() =>
        {
            var grid = new Grid
            {
                ColumnSpacing = 1,
                RowSpacing = 1,
                BackgroundColor = Colors.Black,
                ColumnDefinitions =
        {
            new ColumnDefinition { Width = 100 },
            new ColumnDefinition { Width = 100 },
            new ColumnDefinition { Width = 80 },
            new ColumnDefinition { Width = 80 },
            new ColumnDefinition { Width = 80 },
            new ColumnDefinition { Width = 150 },
            new ColumnDefinition { Width = 120 },
            new ColumnDefinition { Width = GridLength.Auto }
        }
            };

            // Gridに入れるラベルを先に作成
            var playedDeckNameLabel = new Label();
            playedDeckNameLabel.SetBinding(Label.TextProperty, "PlayedDeckName");

            var opponentsDeckNameLabel = new Label();
            opponentsDeckNameLabel.SetBinding(Label.TextProperty, "OpponentsDeckName");

            var coinLabel = new Label();
            coinLabel.SetBinding(Label.TextProperty, "Coin");

            var turnOrderLabel = new Label();
            turnOrderLabel.SetBinding(Label.TextProperty, "TurnOrder");

            var resultLabel = new Label();
            resultLabel.SetBinding(Label.TextProperty, "Result");

            var memoLabel = new Label();
            memoLabel.SetBinding(Label.TextProperty, "Memo");

            var formattedDateTimeLabel = new Label();
            formattedDateTimeLabel.SetBinding(Label.TextProperty, "FormattedDateTime");

            grid.Add(playedDeckNameLabel, 0, 0);
            grid.Add(opponentsDeckNameLabel, 1, 0);
            grid.Add(coinLabel, 2, 0);
            grid.Add(turnOrderLabel, 3, 0);
            grid.Add(resultLabel, 4, 0);
            grid.Add(memoLabel, 5, 0);
            grid.Add(formattedDateTimeLabel, 6, 0);
            var button = new Button
            {
                Text = "削除",
                Style = (Style)Resources["buttonStyle"]
            };
            button.SetBinding(Button.CommandProperty, new Binding("BindingContext.DeleteMatchCommand", source: this));
            button.SetBinding(Button.CommandParameterProperty, new Binding("."));
            grid.Add(button, 7, 0);
            return grid;
        });

#if ANDROID || IOS
        HeaderGrid.IsVisible = false;
#endif
        // プラットフォームごとにテンプレートを切り替え
#if ANDROID
        ResultListView.ItemTemplate = androidTemplate;
#elif WINDOWS
        ResultListView.ItemTemplate = windowsTemplate;
#else
        ResultListView.ItemTemplate = androidTemplate; // デフォルト
#endif
    }
}
