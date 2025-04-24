namespace YuGiOhResult
{
    public partial class App : Application
    {
        public App()
        {

            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell())
            {
                Title = "MD対戦履歴",
                Width = 1000,
                Height = 700,
                X = 50,
                Y = 50
            };
        }
    }
}