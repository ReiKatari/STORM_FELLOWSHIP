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
                var sb = new System.Text.StringBuilder();
                Exception? ex = args.Exception;
                while (ex != null)
                {
                    sb.AppendLine($"[ERROR] {ex.GetType().FullName}: {ex.Message}");
                    sb.AppendLine($"Stack:\n{ex.StackTrace}\n");
                    ex = ex.InnerException;
                }
                System.IO.File.WriteAllText(log, sb.ToString());
                MessageBox.Show(sb.ToString(), "STORM FELLOWSHIP Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch { }
        };
    }
}
