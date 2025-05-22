using System.ComponentModel;
using System.Timers;
using AvaTerminal3.Models.Static;
using Timer = System.Timers.Timer;

namespace AvaTerminal3.Views.Components;

public partial class BottomClockBar : ContentView, INotifyPropertyChanged
{
    private Timer? _timer;

    public string LocalTime => $"🕒 Local: {DateTime.Now:HH:mm:ss}";
    public string UtcTime => $"🌍 UTC: {DateTime.UtcNow:HH:mm:ss}";

    public string AppVersionInfo { get; } = $"v{AppVersion.VersionInfo}";

    public BottomClockBar()
    {
        InitializeComponent();
        BindingContext = this;
    }

    protected override void OnParentSet()
    {
        base.OnParentSet();

        if (_timer == null && Parent != null)
        {
            try
            {
                _timer = new Timer(1000);
                _timer.Elapsed += OnTimerElapsed;
                _timer.AutoReset = true;

                Dispatcher.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    _timer?.Start();
                    return false; // Run once
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BottomClockBar Init Error] {ex.Message}");
            }
        }
    }

    private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        try
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                OnPropertyChanged(nameof(LocalTime));
                OnPropertyChanged(nameof(UtcTime));
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ClockBar Timer Error] {ex.Message}");
            _timer?.Stop();
        }
    }

    public new event PropertyChangedEventHandler? PropertyChanged;

    protected override void OnPropertyChanged(string propertyName)
    {
        base.OnPropertyChanged(propertyName);
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
