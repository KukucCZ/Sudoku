using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace Sudoku.Other
{
    class DifficultyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case Models.Difficulty.Easy:
                    return 0;
                case Models.Difficulty.Medium:
                    return 1;
                case Models.Difficulty.Hard:
                    return 2;
                case Models.Difficulty.Unsolvable:
                    return 3;
            }
            return 3;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (int.Parse(value.ToString()))
            {
                case 0:
                    return Models.Difficulty.Easy;
                case 1:
                    return Models.Difficulty.Medium;
                case 2:
                    return Models.Difficulty.Hard;
                case 3:
                    return Models.Difficulty.Unsolvable;
            }
            return Models.Difficulty.Unsolvable;
        }
    }
}
