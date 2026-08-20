using System.Windows;

namespace StormFellowship;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        this.DispatcherUnhandledException += (s, args) =>
        {
            try
            {
                string log = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "crash.log");
                System.IO.File.WriteAllText(log, $"[FATAL] {DateTime.Now}\n{args.Exception}\nStack: {args.Exception.StackTrace}");
                MessageBox.Show($"STORM FELLOWSHIP Error:\n{args.Exception.Message}\n\n{args.Exception.StackTrace}", "STORM FELLOWSHIP Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch { }
        };
    }
}
