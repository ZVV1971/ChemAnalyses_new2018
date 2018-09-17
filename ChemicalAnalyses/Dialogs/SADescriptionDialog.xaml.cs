using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChemicalAnalyses.Dialogs
{
    public partial class SADescriptionDialog : Window
    {
        private int errorCount = 0;

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { if (value != null) SetValue(DescriptionProperty, value); }
        }

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register(nameof(Description),
                typeof(string), typeof(SADescriptionDialog), 
                new PropertyMetadata("Введите описание анализа не длинее 100 символов!"),
                new ValidateValueCallback(validateDescriptionValue));

        public SADescriptionDialog()
        {
            InitializeComponent();
            grdMain.DataContext = this;
        }

        static bool validateDescriptionValue(object value)
        {
            if (value == null) return false;
            return ((string)value).Length <= 100;
        }

        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        { DialogResult = true; }

        private void SaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        { e.CanExecute = errorCount == 0; }

        private void Window_ValidationError(object sender, ValidationErrorEventArgs e)
        {
            var validationEventArgs = e as ValidationErrorEventArgs;
            if (validationEventArgs == null) throw new Exception("Unexpected event args");
            switch (validationEventArgs.Action)
            {
                case ValidationErrorEventAction.Added:
                    { errorCount++; break; }
                case ValidationErrorEventAction.Removed:
                    { errorCount--; break; }
                default:
                    throw new Exception("Unknown action");
            }
        }
    }
}