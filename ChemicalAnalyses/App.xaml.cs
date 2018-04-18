using System.Windows;
using System.Diagnostics;
using System.Threading;
using System;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace ChemicalAnalyses
{
    public partial class App : Application
    {
        private const int MINIMUM_SPLASH_TIME = 1500; // Miliseconds  

        protected override void OnStartup(StartupEventArgs e)
        {// check for config file presence
            try
            {
                Uri UriAssemblyFolder = new Uri(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase));
                string appPath = UriAssemblyFolder.LocalPath;
                Configuration config = ConfigurationManager.OpenExeConfiguration(appPath + @"\ChemicalAnalyses.exe");
                ClientSettingsSection elementsWeights = (ClientSettingsSection)config.SectionGroups["applicationSettings"].Sections[0];
                elementsWeights.Settings.Get("Mg");
                StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
            }
            catch
            {
                //config file doesn't exist...
                MessageBox.Show("Файл конфигурации не найден. \n Программа не может работать!",
                    "Ошибка файла конфигурации!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                Shutdown(-15);
                return;
            }

            SplashScreen splash = new SplashScreen("CASplashScreen.png");
            splash.Show(false);
            // Step 2 - Start a stop watch  
            Stopwatch timer = new Stopwatch();
            timer.Start();

            // Step 3 - Load your windows but don't show it yet  
            base.OnStartup(e);
            MainWindow main = new MainWindow();
            timer.Stop();

            int remainingTimeToShowSplash = MINIMUM_SPLASH_TIME - (int)timer.ElapsedMilliseconds;
            if (remainingTimeToShowSplash > 0) //if the loading took less time than was planned to show splash
                Thread.Sleep(remainingTimeToShowSplash); //sleep a little bit more

            splash.Close(new TimeSpan(20000000)); //two seconds fade away
        }
    }
}
//ByIDSelectiontype