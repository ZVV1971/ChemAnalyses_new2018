using System;
using System.Globalization;
using System.Windows.Data;
using System.Linq;
using SA_EF;
using System.Windows;

namespace PrintHelper
{
    public class BindingProxy : Freezable
    {
        public static readonly DependencyProperty DataProperty =
           DependencyProperty.Register("Data", typeof(object),
              typeof(BindingProxy), new UIPropertyMetadata(null));

        public object Data
        {
            get { return GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }
        #region Overrides of Freezable
        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }
        #endregion
    }

    [ValueConversion(typeof(decimal), typeof(decimal))]
    public class doubleToPercentageConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("Backward conversion is not possible");
        }

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value is null) return null;
            return (decimal)value * 100;
        }
    }

    public class DecimalsToSumConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null) return null;
            decimal res = 0;
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] != null)
                {
                    try
                    {
                        res += (decimal)values[i];
                    }
                    catch { }
                }
            }
            return res * 100;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("Backward conversion is not possible");
        }
    }

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value != null && (Visibility)value == Visibility.Visible) return true;
            else return false;
        }

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value is null) return Visibility.Hidden;
            return ((bool)value) ? (Visibility.Visible) : (Visibility.Collapsed);
        }
    }

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanToVisibilityNegativeConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value == null || (Visibility)value != Visibility.Visible) return true;
            else return false;
        }

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value is null || !(bool)value) return Visibility.Visible;
            else return Visibility.Collapsed;
        }
    }

    [ValueConversion(typeof(SaltCalculationSchemes), typeof(Visibility),
        ParameterType = typeof(SaltCalculationSchemes))]
    public class SchemeToVisibilityConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("Backward conversion is not possible");
        }

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null) return Visibility.Collapsed;
            if ((SaltCalculationSchemes)value == (SaltCalculationSchemes)parameter) return Visibility.Visible;
            return Visibility.Collapsed;
        }
    }

    [ValueConversion(typeof(object[]), typeof(Visibility))]
    public class SchemeToVisibilityMultipleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values[0] == null) return Visibility.Hidden;
            if (values.Skip(1).Any(p => (SaltCalculationSchemes)(values[0]) == (SaltCalculationSchemes)p))
                return Visibility.Visible;
            return Visibility.Hidden;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("Backward conversion is not possible");
        }
    }
}