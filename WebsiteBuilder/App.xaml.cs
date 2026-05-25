namespace WebsiteBuilder;

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
            Title = "Website Builder",
            Width = 1100,
            Height = 750
        };
    }
}
