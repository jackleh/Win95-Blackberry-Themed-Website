namespace WebsiteBuilder;

public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;

    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var shell = _serviceProvider.GetRequiredService<AppShell>();
        return new Window(shell)
        {
            Title = "Website Builder",
            Width = 1100,
            Height = 750
        };
    }
}
