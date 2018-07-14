using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
//using SaltAnalysisDatas;
using SA_EF;

namespace ChemicalAnalyses.Dialogs
{
    public partial class SaltAnalysisOptionsDlg : Window
    {
        public SaltAnalysisData sa_local { get; set; }
        private int errorCount = 0;

        public SaltAnalysisOptionsDlg(SaltAnalysisData sa)
        {
            sa_local = sa;
            InitializeComponent();
            grdMain.DataContext = sa_local;
        }

        private void Window_ValidationError(object sender, ValidationErrorEventArgs e)
        {
            var validationEventArgs = e as ValidationErrorEventArgs;
            if (validationEventArgs == null) throw new Exception("Unexpected event args");
            switch (validationEventArgs.Action)
            {
                case ValidationErrorEventAction.Added:
                    {errorCount++; break;}
                case ValidationErrorEventAction.Removed:
                    {errorCount--; break;}
                default:
                    throw new Exception("Unknown action");
            }
        }

        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {DialogResult = true;}

        private void SaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {e.CanExecute = errorCount == 0;}
    }
}