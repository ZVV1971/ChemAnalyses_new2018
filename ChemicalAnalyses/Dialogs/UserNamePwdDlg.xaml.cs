using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ChemicalAnalyses.Dialogs
{
    public partial class UserNamePwdDlg : Window
    {
        private int errorCount = 0;

        public string UserName
        {
            get { return (string)GetValue(UserNameProperty); }
            set { if (value != null) SetValue(UserNameProperty, value); }
        }

        public static readonly DependencyProperty UserNameProperty =
            DependencyProperty.Register(nameof(UserName),
                typeof(string), typeof(UserNamePwdDlg),
                new PropertyMetadata("Введите имя пользователя"),
                new ValidateValueCallback(validateUserNameValue));

        public bool wrongPwd
        {
            get { return (bool)GetValue(wrongPwdProperty); }
            set { SetValue(wrongPwdProperty, value); }
        }

        public static readonly DependencyProperty wrongPwdProperty =
            DependencyProperty.Register(nameof(wrongPwd),
                typeof(bool), typeof(UserNamePwdDlg),
                new PropertyMetadata(true));

        public UserNamePwdDlg()
        {
            InitializeComponent();
            grdMain.DataContext = this;
        }

        private void pbPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            wrongPwd = pbPassword.Password.Length < 3;
        }

        static bool validateUserNameValue(object value)
        {
            if (value == null || ((string)value).Equals(string.Empty)) return false;
            return (((string)value).Length >= 3 && ((string)value).Length <= 100);
        }

        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        { DialogResult = true; }

        private void SaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        { e.CanExecute = errorCount == 0 && !wrongPwd; }

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