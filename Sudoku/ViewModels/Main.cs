using System;
using System.Collections.Generic;
using System.Text;
using Sudoku.Models;

namespace Sudoku.ViewModels
{
    class Main
    {
        Solver solver;
        public Main()
        {
            solver = new Solver(new Validator(new Grid()));
            solver.solve();
        }
    }
}
