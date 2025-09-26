using System.Configuration;
using System.Data;
using System.Windows;
using WpfImageClassification.ViewModel;

namespace WpfImageClassification
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow mainWindow = new MainWindow();
            mainWindow.DataContext = new MainWindowViewModel();
            mainWindow.Show();
        }
    }

}
