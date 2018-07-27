using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ChemicalAnalyses.Alumni;

namespace ChemicalAnalyses.Dialogs
{
    public partial class SamplesFilterDlg : Window
    {
        private int errorCount = 0;

        public SampleFilterFields Filter
        {
            get { return (SampleFilterFields)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.Register(nameof(Filter), typeof(SampleFilterFields), typeof(SamplesFilterDlg),
                new PropertyMetadata(null));

        public SamplesFilterDlg()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = errorCount == 0;
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {DialogResult = true;}

        private void Window_Error(object sender, ValidationErrorEventArgs e)
        {
            var validationEventArgs = e as ValidationErrorEventArgs;
            if (validationEventArgs == null) throw new Exception("Unexpected event args");
            switch (validationEventArgs.Action)
            {
                case ValidationErrorEventAction.Added:
                    {
                        errorCount++; break;
                    }
                case ValidationErrorEventAction.Removed:
                    {
                        errorCount--; break;
                    }
                default:
                    {
                        throw new Exception("Unknown action");
                    }
            }
        }
    }
}