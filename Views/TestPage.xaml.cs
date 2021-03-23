using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sudoku.Views
{
    /// <summary>
    /// Interakční logika pro TestPage.xaml
    /// </summary>
    public partial class TestPage : Page
    {
        public TestPage()
        {

            InitializeComponent();

            //Generating Cells
            int xIndex = -1;
            for (int x = 1; x < 12; x++)
            {
                xIndex++;
                if (x == 4 || x == 8 || x == 12) x++;
                int yIndex = -1;
                for (int y = 1; y < 12; y++)
                {
                    yIndex++;
                    if (y == 4 || y == 8 || y == 12) y++;
                    TextBox textBox = new TextBox();
                    Binding binding = new Binding("Cell[" + xIndex + "-" + yIndex + "]");
                    binding.Mode = BindingMode.TwoWay;
                    binding.Converter = new Other.GridConverter();
                    binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                    textBox.SetBinding(TextBox.TextProperty, binding);
                    textBox.SetValue(Grid.RowProperty, x);
                    textBox.SetValue(Grid.ColumnProperty, y);

                    textBox.Margin = new Thickness(-0.1);
                    textBox.VerticalAlignment = VerticalAlignment.Stretch;
                    textBox.HorizontalAlignment = HorizontalAlignment.Stretch;


                    textBox.Name = "Cell" + xIndex + yIndex;

                    playGrid.Children.Add(textBox);
                }
            }
        }
    }
}
