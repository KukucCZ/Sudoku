using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Sudoku.Models
{
    class Generator
    {
        private CellsGrid grid;
        private Random random;
        public CellsGrid Generated { get; private set; }
        public Generator()
        {
            Generated = new CellsGrid();
            grid = new CellsGrid();
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

            Generated = grid.TrueClone();

            //Switches random numbers
            for (int i = 0; i < 9; i++) 
            {
                int num1 = Next();
                int num2 = Next();
                while (num1 == num2) num2 = Next();
                NumberSwitch(num1, num2);
            }
        }

        /// <summary>
        /// Generates sudoku into Generated property.
        /// </summary>
        /// <param name="difficulty">Easy = 0, Medium = 1, Hard = 2</param>
        public void Generate(int difficulty)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Task[] tasks = new Task[4];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Factory.StartNew(() => GenerateHelp(difficulty));
                //tasks[i] = Task.Factory.StartNew(() => GenerateHelp2(difficulty));
            }

            Task.WhenAny(tasks).Wait();
            stopwatch.Stop();
            Debug.Print(stopwatch.ElapsedMilliseconds.ToString());
        }

        /// <summary>
        /// Generates sudoku into Generated property.
        /// </summary>
        /// <param name="difficulty">Relative difficulty of generated sudoku</param>
        /// <param name="offset">Subtract from left numbers to create harder sudoku, CPU intensive</param>
        private void GenerateHelp(int difficulty, int offset = 0)
        {
            int left = 20;
            switch (difficulty)
            {
                case 0:
                    left = 25 - offset;
                    break;
                case 1:
                    left = 24 - offset;
                    break;
                case 2:
                    left = 24 - offset;
                    break;
            }
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            CellsGrid gridTested = grid.TrueClone();
            CellsGrid gridSolved = grid.TrueClone();
            List<List<int>> NotNullList;
            int resets = 1;
            int iterations = 0;
            int last = 0;
            int backDebug = 0;
            Solver solver = new Solver(gridTested);
            while (gridSolved.Count > left && resets < 30)
            {
                if (Generated.Count <= left) return;    //Stops generating if any other thread generated

                iterations++;
                NotNullList = NotNull(gridTested);
                List<int> item = NotNullList[random.Next(NotNullList.Count)];
                gridTested[item[0], item[1]] = 0;
                
                solver.Grid = gridTested.TrueClone();

                switch (difficulty)
                {
                    case 0:
                        //solver.SolveAll(true, true, 4, 4);
                        solver.SolveEasy();
                        break;
                    case 1:
                        //solver.SolveAll(true, true, 0, 4);
                        solver.SolveMedium();
                        break;
                    case 2:
                        //solver.SolveAll(true, true, 2, 0);
                        solver.SolveHard();
                        break;
                }


                if (solver.IsSolved())
                {
                    gridSolved = gridTested.TrueClone();
                    last = 0;
                }
                else
                {
                    gridTested = gridSolved.TrueClone();
                    backDebug++;
                    last++;
                    if (last > gridSolved.Count/2)
                    {
                        Debug.Print(resets.ToString() + "\t" + backDebug.ToString() + "\t" + iterations.ToString() + "\t" + gridSolved.Count.ToString() + "\t" + stopwatch.ElapsedMilliseconds.ToString());
                        stopwatch.Restart();
                        backDebug = 0;
                        iterations = 0;
                        resets++;
                        gridTested = grid.TrueClone();
                        gridSolved = grid.TrueClone();
                    }
                }
            }
            Generated = gridSolved.TrueClone();
            stopwatch.Stop();
            Debug.Print(resets.ToString() + "\t" + backDebug.ToString() + "\t" + iterations.ToString() + "\t" + gridSolved.Count.ToString() + "\t" + stopwatch.ElapsedMilliseconds.ToString());
        }

        /// <summary>
        /// Tries to generate random sudoku. Older algorithm.
        /// </summary>
        /// <param name="difficulty">Dificulty parameter</param>
        private void GenerateHelp2(int difficulty)
        {
            int left = 20;
            switch (difficulty)
            {
                case 0:
                    left = 25;
                    break;
                case 1:
                    left = 24;
                    break;
                case 2:
                    left = 24;
                    break;
            }
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int reset = 0;
            CellsGrid gridTested = grid.TrueClone();
            Solver solver = new Solver(gridTested);
            while (reset < 100)
            {
                reset++;
                while(gridTested.Count > left)
                {
                    gridTested[Next(), Next()] = 0;
                }
                solver.Grid = gridTested.TrueClone();
                switch (difficulty)
                {
                    case 0:
                        solver.SolveAll(true, true, 4, 4);
                        //solver.SolveEasy();
                        break;
                    case 1:
                        solver.SolveAll(true, true, 0, 4);
                        //solver.SolveMedium();
                        break;
                    case 2:
                        solver.SolveAll(true, true, 4, 0);
                        //solver.SolveHard();
                        break;
                }
                if (solver.IsSolved()) break;
                gridTested = grid.TrueClone();
                Debug.Print(reset.ToString() + "\t" + stopwatch.ElapsedMilliseconds.ToString());
            }
            Generated = gridTested.TrueClone();
            stopwatch.Stop();
            Debug.Print(reset.ToString() + "\t" + stopwatch.ElapsedMilliseconds.ToString());
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

        /// <summary>
        /// Switches two numbers in grid.
        /// </summary>
        /// <param name="firstNumber">First number</param>
        /// <param name="secondNumber">Second number</param>
        private void NumberSwitch(int firstNumber, int secondNumber)
        {
            NumberSwitchHelp(firstNumber, 0);
            NumberSwitchHelp(secondNumber, firstNumber);
            NumberSwitchHelp(0, secondNumber);
        }

        /// <summary>
        /// Helper method. Finds and switches two numbers in grid.
        /// </summary>
        /// <param name="firstNumber">First number</param>
        /// <param name="secondNumber">Second number</param>
        private void NumberSwitchHelp(int firstNumber, int secondNumber)
        {
            for (int x = 0; x < 9; x++) for (int y = 0; y < 9; y++)
                {
                    if (firstNumber == grid[x, y]) grid[x, y] = secondNumber;
                }
        }

        /// <summary>
        /// Generates random number between 1 to 9;
        /// </summary>
        /// <returns>Integer between 1 - 9</returns>
        private int Next()
        {
            random = new Random();
            return random.Next(0, 9);
        }

        /// <summary>
        /// Returns list of indexes of input grid, where is not a zero.
        /// </summary>
        /// <param name="grid">Input CellsGrid</param>
        /// <returns>List of Lists with x and y indexes.</returns>
        private List<List<int>> NotNull(CellsGrid grid)
        {
            List<List<int>> tempList = new List<List<int>>();
            for (int x = 0; x < 9; x++) for (int y = 0; y < 9; y++)
                {
                    if (grid[x, y] != 0) tempList.Add(new List<int> { x, y });
                }
            return tempList;
        }
    }
}
