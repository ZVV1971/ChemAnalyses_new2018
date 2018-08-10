using System;
using System.Globalization;
using System.Windows.Data;
using SA_EF;
using System.Windows;
using System.Linq;
using System.Collections.Generic;

namespace ChemicalAnalyses.Alumni
{
    [ValueConversion(typeof(string),typeof(bool))]
    public class StringToBooleanConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("Backward conversion is not possible");
        }

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if ((string)value == "Not filtered") return false;
            return true;
        }
    }

    [ValueConversion(typeof(object[]),typeof(bool))]
    public class SampleAvConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string labnm;
            string descr;
            DateTime smplDate;
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";
            
            try
            {
                labnm = values[0].ToString().Trim();
                smplDate = DateTime.Parse(values[1].ToString());
                descr = values[2].ToString().Trim();
                Sample smpl = new Sample
                {
                    IDSample =1, //Dummy just to test the other fields
                    LabNumber = labnm,
                    SamplingDate = smplDate,
                    Description = descr
                };
                return true;
            }
            catch { return false; }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Обратная конвертация невозможна!");
        }
    }

    [ValueConversion(typeof(string), typeof(double?))]
    public class StringToNullableDoubleConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            double d;
            object o;
            string sep = culture.NumberFormat.NumberDecimalSeparator;
            if (value.ToString().Trim() == "") return null;
            try
            {
                d = double.Parse(value.ToString());
                return o = d;
            }
            catch
            {
                return null;
            }
        }

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value == null) return "";
            return value.ToString();
        }
    }

    [ValueConversion(typeof(decimal), typeof(decimal))]
    public class DoubleToPercentageConverter : IValueConverter
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

    [ValueConversion(typeof(bool), typeof(bool))]
    public class BooleanToNegatedBooleanConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        { return !(bool)value; }

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        { return !(bool)value; }
    }

    [ValueConversion(typeof(bool), typeof(string))]
    public class BooleanToUserTypeConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return false;
        }

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value is null) return "Пользователь";
            return ((bool)value) ? ("Администратор") : ("Пользователь");
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
        ParameterType =typeof(SaltCalculationSchemes))]
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

    [ValueConversion(typeof(object[]),typeof(Visibility))]
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

    [ValueConversion(typeof(SaltCalculationSchemes), typeof(string))]
    public class SchemeToSchemeDescriptionConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("Backward conversion not implemented");
        }

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value != null) return ((SaltCalculationSchemes)value).ToName();
            return null;
        }
    }

    [ValueConversion(typeof(SaltCalculationSchemes), typeof(KeyValuePair<SaltCalculationSchemes, string>))]
    public class SchemeToSchemeDescriptionKVPairConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value !=null)
            return ((KeyValuePair<SaltCalculationSchemes, string>)value).Key;
            return null;
        }

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value != null) return new KeyValuePair<SaltCalculationSchemes,string>
               ((SaltCalculationSchemes)value, ((SaltCalculationSchemes)value).ToName());
            return null;
        }
    }

    [ValueConversion(typeof(string), typeof(KeyValuePair<string, string>))]
    public class ElementToElementDescriptionKVPairConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value != null)
                return ((KeyValuePair<string, string>)value).Key;
            return null;
        }

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            try
            {
                if (value != null) return new KeyValuePair<string, string>
                   ((string)value,
                   ((ChemicalElemetCalibration)Enum.Parse(typeof(ChemicalElemetCalibration),
                        ((string)value).Trim())).ToName());
            }
            catch { return null; }
            return null;
        }
    }
}