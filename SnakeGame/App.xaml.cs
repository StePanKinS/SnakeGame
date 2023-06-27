using System;
using System.Threading;
using System.Windows;

namespace SnakeGame
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        Mutex mutex;

        public App()
        {
            mutex = new Mutex(true, "myTextApp", out bool gotMutex);
            if(!gotMutex)
            {
                MessageBox.Show("Already runs");
                Shutdown();

                return;
            }
            
            

            Exit += App_Exit;
            LoadCompleted += App_LoadCompleted;

            StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);

            
        }

        private void App_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            MessageBox.Show("Load Complited");
        }

        //[STAThread]
        //public static void Main(string[] args)
        //{
        //    App app = new App();

        //    app.Run();
        //}

        private void App_Exit(object sender, ExitEventArgs e)
        {
            GameSettings.SaveSettings();
            e.ApplicationExitCode = 0;
        }
    }
}
