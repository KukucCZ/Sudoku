using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Sudoku.Models
{
    class Generator
    {
        private CellsGrid grid;
        private Solver solver;
        private Random random;
        public CellsGrid Generated { get; private set; }
        public Generator()
        {
            grid = new CellsGrid();
            solver = new Solver(grid);
            random = new Random();
            
            XElement element;
            using (Stream reader = new FileStream("..\\..\\..\\Data\\solved.xml", FileMode.Open))
            {
                element = XElement.Load(reader);
            }

            var tempList = element.Descendants("sudoku").ToList().Select(d => { d.Value.Trim(); return d.Value.Split("\n").Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x.ToString())); })
                .ToArray().Select(x => x.Select(y => y.Split(" ").Select(d => int.Parse(d)))).ToArray();

            var temp = tempList[random.Next(tempList.Length)];

            int x = 0;
            int y = 0;
            foreach (var item in temp)
            {
                if (x > 8) x = 0;
                foreach (var num in item)
                {
                    if (y > 8) y = 0;
                    grid[x, y] = num;
                    y++;
                }
                x++;
            }

            DebugGridPrint();
        }

        public void Generate(int difficulty)
        {
            switch (difficulty)
            {
                case 0:
                    GenerateEasy();
                    break;
                case 1:
                    GenerateMedium();
                    break;
                case 2:
                    GenerateHard();
                    break;
                default:
                    break;
            }
        }
        private void GenerateEasy()
        {
            CellsGrid tempGrid = new CellsGrid();
            int iterations = 0;
            int tests = 0;
            for(; ; )
            {
                grid = new CellsGrid();
                tempGrid = new CellsGrid();
                while (tempGrid.Count < 20)
                {
                    DebugGridPrint();
                    iterations++;
                    tempGrid = grid.Clone();
                    tempGrid[random.Next(0, 9), random.Next(0, 9)] = random.Next(1, 9);
                    if (new Validator(tempGrid).IsValid()) grid = tempGrid.Clone();
                }
                if (new Solver(grid).SolveHard()) break;
                tests++;
                Debug.Print(tests.ToString());
            }
            Generated = grid.Clone();
        }
        private void GenerateMedium()
        {

        }
        private void GenerateHard()
        {

        }

        //Debug printing method
        private void DebugGridPrint(string message = "")
        {
            DebugGridPrintMain(grid, message);
        }

        private void DebugGridPrintMain(CellsGrid cells, string message = "", bool print = true)
        {
            if (!print) return;
            string helper = "";
            Debug.Print(message);
            for (int x = 0; x < 9; x++)
            {
                helper = "";
                for (int y = 0; y < 9; y++)
                {
                    if (y % 3 == 0) helper += "| ";
                    if (cells[x, y] == 0) helper += "  ";
                    else helper += cells[x, y].ToString() + " ";
                }
                if (x % 3 == 0) Debug.Print("------------------------");
                Debug.Print(helper);
            }
            Debug.Print("");
        }
    }
}
