using Hardcodet.Wpf.TaskbarNotification;
using System.Windows;
using System.Windows.Input;

namespace WindowsGPT
{
  public partial class App : Application
  {
    private TaskbarIcon _notifyIcon;
    public MainWindowViewModel ViewModel { get; set; }

    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);
      ViewModel = new MainWindowViewModel();

      _notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
    }

    protected override void OnExit(ExitEventArgs e)
    {
      _notifyIcon.Dispose(); //the icon would clean up automatically, but this is cleaner
      base.OnExit(e);
    }

  }

}
