using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace Sudoku.Other
{
    class GridConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string final = value.ToString();
            if (final == "0") return "";
            else return final;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!int.TryParse((string)value, out int final))
            {
                if (value.ToString().Length > 0) value = value.ToString().Remove(value.ToString().Length - 1);
                if (!int.TryParse((string)value, out final)) return 0;
            }
            if (final > 0 && final < 10) return final;
            return final % 10;
        }
    }
}
