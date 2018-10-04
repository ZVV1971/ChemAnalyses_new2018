using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

namespace ChemicalAnalyses
{
    public partial class App : Application
    {
        private const int MINIMUM_SPLASH_TIME = 1500; // Miliseconds  

        // Use a name unique to the application (including GUID)
        private static Mutex mutex = new Mutex(false, 
            @"ZVV_Diploma_Mutex/{DF776A4B-389C-4A4F-AD0B-1BE989F11ED9}", out mutexIsCreated);
        private static bool mutexIsCreated;

        protected override void OnStartup(StartupEventArgs e)
        {
            if (!mutex.WaitOne(TimeSpan.FromSeconds(1), false))
            {
                MessageBox.Show("Запущена еще одна копия приложения.", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                //And post message (WM_SHOWME) to the running instance
                NativeMethods.PostMessage((IntPtr)NativeMethods.HWND_BROADCAST,
                    NativeMethods.WM_SHOWME, IntPtr.Zero, IntPtr.Zero);
                Shutdown(-2);
                return;
            }

                // check for config file presence
                Configuration config;
            try
            {
                Uri UriAssemblyFolder = new Uri(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase));
                string appPath = UriAssemblyFolder.LocalPath;
                config = ConfigurationManager.OpenExeConfiguration(appPath + @"\ChemicalAnalyses.exe");
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

            SplashScreen splash = new SplashScreen("SASplashScreen.png");
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

            splash.Close(TimeSpan.FromSeconds(1)); //one second fade away
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            try { if (mutexIsCreated) mutex.ReleaseMutex(); }
            catch (Exception ex){ }
        }
    }

    internal class NativeMethods
    {
        public const int HWND_BROADCAST = 0xffff;
        public static readonly int WM_SHOWME = RegisterWindowMessage("WM_SHOWME");
        [DllImport("user32")]
        public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);
        [DllImport("user32")]
        public static extern int RegisterWindowMessage(string message);
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int cmdShow);
    }
}