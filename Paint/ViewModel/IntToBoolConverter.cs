using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Paint.ViewModel
{
    [ValueConversion(typeof(string), typeof(int))]
    public class IntToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type t, object parameter, CultureInfo culture)
        {
            return value.Equals(parameter);
        }
        public object ConvertBack(object value, Type t, object parameter, CultureInfo culture)
        {
            return value.Equals(false) ? DependencyProperty.UnsetValue : parameter;
        }
    }
}
