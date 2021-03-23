using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sudoku.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Moves focus between TextBoxes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                TraversalRequest tRequest = new TraversalRequest(FocusNavigationDirection.Up);
                UIElement keyboardFocus = Keyboard.FocusedElement as UIElement;

                if (keyboardFocus != null)
                {
                    keyboardFocus.MoveFocus(tRequest);
                }
                TextBox text = (TextBox)Keyboard.FocusedElement;
                text.CaretIndex = text.Text.Length;
                e.Handled = true;
            }
            else if (e.Key == Key.Down)
            {
                TraversalRequest tRequest = new TraversalRequest(FocusNavigationDirection.Down);
                UIElement keyboardFocus = Keyboard.FocusedElement as UIElement;

                if (keyboardFocus != null)
                {
                    keyboardFocus.MoveFocus(tRequest);
                }
                TextBox text = (TextBox)Keyboard.FocusedElement;
                text.CaretIndex = text.Text.Length;
                e.Handled = true;
            }
            else if (e.Key == Key.Left)
            {
                TraversalRequest tRequest = new TraversalRequest(FocusNavigationDirection.Left);
                UIElement keyboardFocus = Keyboard.FocusedElement as UIElement;

                if (keyboardFocus != null)
                {
                    keyboardFocus.MoveFocus(tRequest);
                }
                TextBox text = (TextBox)Keyboard.FocusedElement;
                text.CaretIndex = text.Text.Length;
                e.Handled = true;
            }
            else if (e.Key == Key.Right)
            {
                TraversalRequest tRequest = new TraversalRequest(FocusNavigationDirection.Right);
                UIElement keyboardFocus = Keyboard.FocusedElement as UIElement;

                if (keyboardFocus != null)
                {
                    keyboardFocus.MoveFocus(tRequest);
                }
                TextBox text = (TextBox)Keyboard.FocusedElement;
                text.CaretIndex = text.Text.Length;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Sets TextBox caret to the last place.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBox text = (TextBox)sender;
            text.Focus();
            text.CaretIndex = text.Text.Length;
            e.Handled = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(Keyboard.FocusedElement is TextBox text)
            {
                Grid grid = (Grid)text.Parent;
                grid.Focusable = true;
                grid.Focus();
                grid.Focusable = false;
            }
        }
    }
}
