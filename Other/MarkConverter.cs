using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Sudoku.Other
{
    class MarkConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {/*
            if (value.ToString() == "0") return new SolidColorBrush(Colors.Transparent);
            return new SolidColorBrush(Colors.Red);*/

            if (value.ToString() == "0") return false;
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
