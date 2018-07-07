using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Reflection;
using ChemicalAnalyses.Dialogs;
using SaltAnalysisDatas;
using SettingsHelper;

namespace ChemicalAnalyses
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                CALogger.InitLogFile(Properties.Settings.Default.LogFile);
            }
            catch
            {
                MessageBox.Show("Ошибка в файле конфигурации");
                Close();
            }
            CALogger.WriteToLogFile("Программа запущена");
            CALogger.WriteToLogFile("Подключение к БД…");
            try
            {
                string UserLevelPath = Properties.Settings.Default.DBFilePath;
                if (ConnectionStringGiver.GetValidConnectionString(UserLevelPath) != null)
                    Properties.Settings.Default.DBFilePath = UserLevelPath;
                else { CALogger.WriteToLogFile("Не найдена БД"); Close(); }
            }
            catch
            {
                CALogger.WriteToLogFile("Ошибка при считывании строки подключения!");
                MessageBox.Show("Ошибка в файле конфигурации");
                Close();
            }
        }

        #region HoverToolTip Property
        public object HoverToolTip
        {
            get { return (object)GetValue(HoverToolTipProperty); }
            set { SetValue(HoverToolTipProperty, value); }
        }

        public static readonly DependencyProperty HoverToolTipProperty =
            DependencyProperty.Register(nameof(HoverToolTip), typeof(object), typeof(MainWindow),
                new PropertyMetadata(null));
        #endregion HoverToolTip Property

        private void Window_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            var element = Mouse.DirectlyOver as FrameworkElement;
            HoverToolTip = GetTooltip(element);
        }

        protected static Object GetTooltip(FrameworkElement obj)
        {
            if (obj == null){ return null; }
            else if (obj.ToolTip != null){ return obj.ToolTip; }
            else{ return GetTooltip(VisualTreeHelper.GetParent(obj) as FrameworkElement); }
        }

        #region Commands
        private void ExitCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Properties.Settings.Default.Save();
            CALogger.WriteToLogFile("Закрытие программы");
            Application.Current.Shutdown();
        }
        private void HelpCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("Курсовая работа по теме \"пока без темы\"\nЗахаренков В.В. группа №60325-2\nВерсия: " +
                Assembly.GetExecutingAssembly().GetName().Version.ToString(), "О программе…",
                        MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ListCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SamplesViewDlg dlg = new SamplesViewDlg();
            if (dlg.ShowDialog() == true)
            {}
        }
        #endregion Commands

        private void CalibrationMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CalibrationSelectionDlg dlg = null;
            string type = ((MenuItem)sender).Header.ToString();
            switch (type)
            {
                case "Калий":
                    dlg = new CalibrationSelectionDlg("Kalium", Properties.Settings.Default.KaliumCalibrationNumber);
                    break;
                case "Натрий":
                    dlg = new CalibrationSelectionDlg("Natrium", Properties.Settings.Default.NatriumCalibrationNumber);
                    break;
                default:
                    break;
            }
            dlg.btnSetDefault.Content = "Установить по умолчанию";
            dlg.btnSetDefault.ToolTip = "Установить выбранную калибровку по умолчанию для всех новых анализов";
            if (dlg.ShowDialog() == true)
            {
                switch (type)
                {
                    case "Калий":
                        Properties.Settings.Default.KaliumCalibrationNumber = dlg.CalibrationNumber;
                        break;
                    case "Натрий":
                        Properties.Settings.Default.NatriumCalibrationNumber = dlg.CalibrationNumber;
                        break;
                    default:
                        break;
                }
            }
        }

        private void SAOptionsMenuItem_Click (object sender, RoutedEventArgs e)
        {
            //use an instance of SaltAnalysis class to check for allowable values
            SaltAnalysisData sa = new SaltAnalysisData();
            //fill values from the user-defined section of settings
            sa.HgCoefficient = Properties.Settings.Default.HgCoefficient;
            sa.BromumStandardTitre = Properties.Settings.Default.BrTitre;
            sa.CalciumTrilonTitre = Properties.Settings.Default.CaTrilonB;
            sa.MagnesiumTrilonTitre = Properties.Settings.Default.MgTrilonB;
            sa.SulfatesBlank = Properties.Settings.Default.SulfatesBlank;
            sa.BromumBlank = Properties.Settings.Default.BrBlank;
            
            SaltAnalysisOptionsDlg saDlg = new SaltAnalysisOptionsDlg(sa);
            if (saDlg.ShowDialog() == true)
            {//if OK save settings back to user.config
                Properties.Settings.Default.HgCoefficient= sa.HgCoefficient;
                Properties.Settings.Default.BrTitre = sa.BromumStandardTitre;
                Properties.Settings.Default.CaTrilonB = sa.CalciumTrilonTitre;
                Properties.Settings.Default.MgTrilonB= sa.MagnesiumTrilonTitre;
                Properties.Settings.Default.SulfatesBlank= sa.SulfatesBlank;
                Properties.Settings.Default.BrBlank= sa.BromumBlank;
            }
        }
    }
}