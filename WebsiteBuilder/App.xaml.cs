namespace WebsiteBuilder;

public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;

    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;

        // Win95 has exactly one look — pin Light so the host OS's dark mode
        // can't repaint Shell surfaces (e.g. the flyout list) with dark defaults.
        UserAppTheme = AppTheme.Light;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var shell = _serviceProvider.GetRequiredService<AppShell>();
        var window = new Window(shell)
        {
            Title = "Website Builder",
            Width = 1100,
            Height = 750
        };

#if WINDOWS
        // Win95 navy title bar with a white caption. Using MAUI's high-level
        // TitleBar (rather than AppWindow.TitleBar colors) gives a single custom
        // bar — no redundant gray strip, and the app name reads clearly.
        window.TitleBar = new TitleBar
        {
            Title = "Website Builder",
            BackgroundColor = Color.FromArgb("#000080"),
            ForegroundColor = Colors.White,
            HeightRequest = 32
        };

        // The min/maximize/close glyphs are OS-drawn and follow the system theme
        // (black in dark mode). Force them white so they read on the navy bar.
        void ForceCaptionButtonsWhite()
        {
            if (window.Handler?.PlatformView is Microsoft.UI.Xaml.Window native)
            {
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(native);
                var id = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
                var tb = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(id).TitleBar;
                var white = global::Windows.UI.Color.FromArgb(255, 255, 255, 255);
                var transparent = global::Windows.UI.Color.FromArgb(0, 0, 0, 0);
                tb.ButtonForegroundColor = white;
                tb.ButtonInactiveForegroundColor = white;
                tb.ButtonHoverForegroundColor = white;
                tb.ButtonPressedForegroundColor = white;
                tb.ButtonBackgroundColor = transparent;
                tb.ButtonInactiveBackgroundColor = transparent;
                tb.ButtonHoverBackgroundColor = global::Windows.UI.Color.FromArgb(255, 16, 132, 208);
                tb.ButtonPressedBackgroundColor = global::Windows.UI.Color.FromArgb(255, 0, 0, 160);
            }
        }
        // Re-apply on a low-priority dispatcher tick too: MAUI's TitleBar restyles
        // the caption buttons per app theme after Activated fires, so applying once
        // synchronously loses the race.
        void ScheduleForceButtons()
        {
            ForceCaptionButtonsWhite();
            if (window.Handler?.PlatformView is Microsoft.UI.Xaml.Window nat)
                nat.DispatcherQueue.TryEnqueue(
                    Microsoft.UI.Dispatching.DispatcherQueuePriority.Low,
                    ForceCaptionButtonsWhite);
        }
        window.Activated += (_, _) => ScheduleForceButtons();
        window.HandlerChanged += (_, _) => ScheduleForceButtons();
#endif

        return window;
    }
}
