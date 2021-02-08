using System;
using System.Collections.Generic;
using System.Text;
using Sudoku.Models;
using Sudoku.Views;

namespace Sudoku.ViewModels
{
    class Main
    {
        Solver solver;
        public Main()
        {
            solver = new Solver(new Validator(new Grid()));
            solver.solveAll(true, true, 2);
            //solver.removeHidden(2, 8, 4);
            MainWindow main = new MainWindow();
            main.Show();
        }
    }
}
