using System;
using System.Windows.Interactivity;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows;

namespace ChemicalAnalyses.Alumni
{
    /// <summary>
    /// When placed into the window XAML switches window style w/o sysmenu ~WS_SYSMENU
    /// </summary>
    public class HideCloseButtonOnWindow : Behavior<Window>
    {
        #region bunch of native methods

        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        #endregion

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += OnLoaded;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Loaded -= OnLoaded;
            base.OnDetaching();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(AssociatedObject).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }
    }
}