using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace Sudoku.Models
{
    class Solver : Validator
    {
        private bool debug = false;
        private List<List<List<int>>> possible;
        public List<CellsGrid> Steps { get; private set; }
        public CellsGrid SolvedGrid { get; private set; }
        public Solver(CellsGrid inputGrid) : base(inputGrid)
        {
            Grid = inputGrid.Clone();
            possible = new List<List<List<int>>>();
            for (int i = 0; i < 9; i++)
            {
                possible.Add(new List<List<int>>());
                for (int j = 0; j < 9; j++)
                {
                    possible[i].Add(new List<int>());
                }
            }
            Steps = new List<CellsGrid>();
            Steps.Add(Grid.Clone());

            //if(Grid.Count > 5) SolveHard();
        }
        
        /// <summary>
        /// Solves a sudoku with easy preset.
        /// </summary>
        /// <returns>True if sudoku was solved.</returns>
        public bool SolveEasy()
        {
            SolveAll(true, false);
            return IsSolved();
        }

        /// <summary>
        /// Solves a sudoku with medium preset.
        /// </summary>
        /// <returns>True if sudoku was solved.</returns>
        public bool SolveMedium()
        {
            SolveAll(true, true, 0, 2);
            return IsSolved();
        }

        /// <summary>
        /// Solves a sudoku with hard preset.
        /// </summary>
        /// <returns>True if sudoku was solved.</returns>
        public bool SolveHard()
        {
            SolveAll(true, true, 4, 4, true);
            return IsSolved();
        }

        /// <summary>
        /// Solves a sudoku with different parameters.
        /// </summary>
        /// <param name="possibleAlone">Use possibleAlone method.</param>
        /// <param name="possiblePair">Use possiblePair method.</param>
        /// <param name="hidden">Use removeHidden method.</param>
        /// <param name="debug">Steps outputing into debugging console.</param>
        /// <returns></returns>
        public int SolveAll(bool possibleAlone = true, bool possiblePair = true, int hidden = 0, int same = 0, bool debug = false)
        {
            this.debug = debug;
            bool changed = false;
            int changedCounter = 0;
            int iteration = 0;
            DebugGridPrint();
            //for (int x = 0; x < 9; x++) for (int y = 0; y < 9; y++) if (Grid[x, y] != 0) possible[x][y].Add(Grid[x, y]);
            for(; ; )
            {
                //Adding numbers to possible.
                for (int x = 0; x < 9; x++) for (int y = 0; y < 9; y++) if (Grid[x, y] == 0) Intersect(x, y);

                //Removing numbers from possible.
                for (int x = 0; x < 9; x++) for (int y = 0; y < 9; y++) if (Grid[x, y] != 0) RemovePossible(x, y);

                if (possiblePair) for (int x = 0; x < 9; x++) for (int y = 0; y < 9; y++) if (Grid[x, y] == 0) RemovePair(x, y);

                if (hidden != 0) for (int x = 0; x < 9; x++) for (int y = 0; y < 9; y++) if (Grid[x, y] == 0)
                            {
                                RemoveHidden(x, y);
                                if (hidden > 2) RemoveHidden(x, y, 3);
                                if (hidden > 3) RemoveHidden(x, y, 4);
                            }
                
                if (same != 0) for (int x = 0; x < 9; x++) for (int y = 0; y < 9; y++) if (Grid[x, y] == 0)
                            {
                                RemoveSame(x, y);
                                if (same > 2) RemoveSame(x, y, 3);
                                if (same > 3) RemoveSame(x, y, 4);
                            }

                //Adding numbers to grid.
                for (int x = 0; x < 9; x++) for (int y = 0; y < 9; y++) if (Grid[x, y] == 0)
                        {
                            if (Count(x, y))
                            {
                                changed = true;
                                DebugGridPrint("COUNT X: " + x + " Y: " + y + " NUM: " + Grid[x, y]);
                                Steps.Add(Grid.Clone());
                            }
                            else if (possibleAlone && AlonePossible(x, y))
                            {
                                changed = true;
                                Steps.Add(Grid.Clone());
                            }
                        }

                //Ends loop if nothing is added 10 iterations in row.
                if (changed) changedCounter = 0;
                else changedCounter++;
                if (changedCounter >= 10) break;
                changed = false;

                if (IsSolved())
                {
                    if (debug)
                    {
                        Debug.Print("Solved");
                        string helper = "";
                        for (int x = 0; x < 9; x++)
                        {
                            helper = "";
                            for (int y = 0; y < 9; y++)
                            {
                                helper += Grid[x, y].ToString() + " ";
                            }
                            Debug.Print(helper);
                        }
                        Debug.Print("");
                    }
                    break;
                }

                iteration++;
            }
            SolvedGrid = Grid.Clone();
            return iteration;
        }

        //Debug printing methods
        private void DebugGridPrint(string message = "")
        {
            DebugGridPrintMain(Grid, message, debug);
        }

        private void DebugGridPrintMain(CellsGrid cells, string message = "", bool print = false)
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
                    if (cells[x, y] == 0) helper +=  "  ";
                    else helper += cells[x, y].ToString() + " ";
                }
                if (x % 3 == 0) Debug.Print("------------------------");
                Debug.Print(helper);
            }
            Debug.Print("");
        }

        private void DebugListPrint(List<int> list)
        {
            string helper = "{";
            foreach (int item in list)
            {
                helper += item + ", ";
            }
            Debug.Print(helper + "}");
        }

        //Main solving methods

        /// <summary>
        /// Adds to cell possibilities only numbers that intersect in row, cell and subgrid.
        /// </summary>
        /// <param name="x">x index of empty cell</param>
        /// <param name="y">y index of empty cell</param>
        private void Intersect(int x, int y)
        {
            List<int> row = MissingNumbersInRow(x);
            List<int> col = MissingNumbersInColumn(y);
            List<int> sub = MissingNumbersInSubgrid(x, y);
            IEnumerable<int> helper = row.Intersect(col.Intersect(sub));
            possible[x][y] = helper.ToList();
        }

        /// <summary>
        /// If only TYPE number of cells in row, column or subgrid have same TYPE number of possible numbers. Then other possible numbers from these cells are removed.
        /// </summary>
        /// <param name="x">x index of empty cell</param>
        /// <param name="y">y index of empty cell</param>
        /// <param name="type">How many possible numbers and cells</param>
        /// <returns>True if anything was removed.</returns>
        public bool RemoveHidden(int x, int y, int type = 2)
        {
            List<List<int>> numbers = new List<List<int>>();
            int n = possible[x][y].Count;
            if (type >= n) return false;

            for (int num1 = 0; num1 < n - 1; num1++)
            {
                for (int num2 = num1 + 1; num2 < n; num2++)
                {
                    if (num1 == num2) continue;
                    if (type > 2)
                    {
                        for (int num3 = num2 + 1; num3 < n; num3++)
                        {
                            if (num2 == num3) continue;
                            if (type > 3)
                            {
                                for (int num4 = num3 + 1; num4 < n; num4++)
                                {
                                    if (num3 == num4) continue;
                                    List<int> num = new List<int> { possible[x][y][num1], possible[x][y][num2], possible[x][y][num3], possible[x][y][num4] };
                                    if (!numbers.Contains(num)) numbers.Add(num);
                                }
                            }
                            else
                            {
                                List<int> num = new List<int> { possible[x][y][num1], possible[x][y][num2], possible[x][y][num3] };
                                if (!numbers.Contains(num)) numbers.Add(num);
                            }
                        }
                    }
                    else
                    {
                        List<int> num = new List<int> { possible[x][y][num1], possible[x][y][num2] };
                        if (!numbers.Contains(num)) numbers.Add(num);
                    }
                }
            }
            foreach (List<int> combination in numbers)
            {
                List<List<int>> row = new List<List<int>>();
                List<List<int>> column = new List<List<int>>();
                List<List<int>> subgrid = new List<List<int>>();

                for (int i = 0; i < 9; i++)
                {
                    bool help = false;
                    for (int r = 0; r < type; r++)
                    {
                        if (GetPossibleRow(x)[i].Contains(combination[r])) help = true;
                    }
                    if (help) row.Add(new List<int> { x, i });

                    help = false;
                    for (int r = 0; r < type; r++)
                    {
                        if (GetPossibleColumn(y)[i].Contains(combination[r])) help = true;
                    }
                    if (help) column.Add(new List<int> { i, y });

                    help = false;
                    for (int r = 0; r < type; r++)
                    {
                        if (GetPossibleSubgrid(x, y)[i].Contains(combination[r])) help = true;
                    }
                    if (help) subgrid.Add(new List<int> { i / 3 + (x / 3) * 3, i % 3 + (y / 3) * 3 });
                }

                if (row.Count == type && subgrid.Count <= type)
                {
                    foreach (List<int> item in row)
                    {
                        possible[item[0]][item[1]] = combination;
                        if (debug) Debug.Print("Hidden row " + item[0] + " " + item[1] + " num " + combination[0] + ", " + combination[1]);
                    }
                    return true;
                }
                if (column.Count == type && subgrid.Count <= type)
                {
                    foreach (List<int> item in column)
                    {
                        possible[item[0]][item[1]] = combination;
                        if (debug) Debug.Print("Hidden col " + item[0] + " " + item[1] + " num " + combination[0] + ", " + combination[1]);
                    }
                    return true;
                }/* Not 100% OK
                if (subgrid.Count == type)
                {
                    foreach (List<int> item in subgrid)
                    {
                        possible[item[0]][item[1]] = combination;
                        Debug.Print("Hidden sub " + item[0] + " " + item[1] + " num " + combination[0] + ", " + combination[1]);
                    }
                    return true;
                }*/
            }
            return false;
        }

        /// <summary>
        /// If only TYPE number of cells in row, column or subgrid have same TYPE number of possible numbers. These numbers are removed from other cells in column or row.
        /// </summary>
        /// <param name="x">x index of empty cell</param>
        /// <param name="y">y index of empty cell</param>
        /// <param name="type">How many possible numbers and cells</param>
        /// <returns>True if anything was removed.</returns>
        public bool RemoveSame(int x, int y, int type = 2)
        {
            List<List<int>> numbers = new List<List<int>>();
            int n = possible[x][y].Count;
            if (type >= n) return false;

            for (int num1 = 0; num1 < n - 1; num1++)
            {
                for (int num2 = num1 + 1; num2 < n; num2++)
                {
                    if (num1 == num2) continue;
                    if (type > 2)
                    {
                        for (int num3 = num2 + 1; num3 < n; num3++)
                        {
                            if (num2 == num3) continue;
                            if (type > 3)
                            {
                                for (int num4 = num3 + 1; num4 < n; num4++)
                                {
                                    if (num3 == num4) continue;
                                    List<int> num = new List<int> { possible[x][y][num1], possible[x][y][num2], possible[x][y][num3], possible[x][y][num4] };
                                    if (!numbers.Contains(num)) numbers.Add(num);
                                }
                            }
                            else
                            {
                                List<int> num = new List<int> { possible[x][y][num1], possible[x][y][num2], possible[x][y][num3] };
                                if (!numbers.Contains(num)) numbers.Add(num);
                            }
                        }
                    }
                    else
                    {
                        List<int> num = new List<int> { possible[x][y][num1], possible[x][y][num2] };
                        if (!numbers.Contains(num)) numbers.Add(num);
                    }
                }
            }
            foreach (List<int> combination in numbers)
            {
                List<int> row = new List<int>();
                List<int> column = new List<int>();
                List<int> subgrid = new List<int>();
                //Debug.Assert(!(x == 1));
                for (int i = 0; i < 9; i++)
                {
                    if (GetPossibleRow(x)[i].All(d => combination.Contains(d))) row.Add(i);
                    if (GetPossibleColumn(y)[i].All(d => combination.Contains(d))) column.Add(i);
                    if (GetPossibleSubgrid(x, y)[i].All(d => combination.Contains(d))) subgrid.Add(i);
                }

                if (row.Count == type)
                {
                    foreach (int num in combination)
                    {
                        for (int i = 0; i < 9; i++)
                        {
                            if (!row.Contains(i))
                            {
                                GetPossibleRow(x)[i].Remove(num);
                                if (debug) Debug.Print("Same row " + x + "; " + i + " num " + num);
                            }
                        }
                    }
                    return true;
                }
                else if (column.Count == type)
                {
                    foreach (int num in combination)
                    {
                        for (int i = 0; i < 9; i++)
                        {
                            if (!column.Contains(i)) 
                            { 
                                GetPossibleColumn(y)[i].Remove(num);
                                if (debug) Debug.Print("Same column " + i +"; " + y + " num " + num);
                            } 
                        }
                    }
                    return true;
                }/* not 100% ok
                else if (subgrid.Count == type)
                {
                    foreach (int num in combination)
                    {
                        for (int i = 0; i < 9; i++)
                        {
                            if (!subgrid.Contains(i)) 
                            { 
                                GetPossibleSubgrid(x, y)[i].Remove(num);
                                if (debug) Debug.Print("Same subgrid " + i/3*3 +"; " + i%3 + " num " + num);
                            } 
                        }
                    }
                    return true;
                }*/
            }
            return false;
        }

        /// <summary>
        /// Adds number if it is only possible number in cell.
        /// </summary>
        /// <param name="x">x index of empty cell</param>
        /// <param name="y">y index of empty cell</param>
        /// <returns>True if number was added.</returns>
        private bool Count(int x, int y)
        {
            if (possible[x][y].Count == 1)
            {
                Grid[x, y] = possible[x][y].First();
                return true;
            }
            return false;
        }

        /// <summary>
        /// If only two or three cells in subgrid have same possible number and all cells are in column or row. Then this number is removed in all other cells in row or column. 
        /// </summary>
        /// <param name="x">x index of empty cell</param>
        /// <param name="y">y index of empty cell</param>
        /// <returns>True if anything was removed.</returns>
        private bool RemovePair(int x, int y)
        {
            //Debug.Assert(!(x == 1 && y == 4));
            bool used = false;
            int xSubgrid = (x / 3) * 3;
            int ySubgrid = (y / 3) * 3;
            foreach (int num in possible[x][y].ToList())
            {
                bool col = false;
                bool row = false;
                int occurrence = CountSubgrid(x, y, num);
                if (occurrence == 2 || occurrence == 3)
                {
                    int countCol = 0;
                    int countRow = 0;
                    for (int i = 0; i < 3; i++)
                    {
                        if (Grid[xSubgrid + i, y] == 0 && possible[xSubgrid + i][y].Contains(num)) countCol++;
                        if (Grid[x, ySubgrid + i] == 0 && possible[x][ySubgrid + i].Contains(num)) countRow++;
                    }
                    if (countCol == occurrence) col = true;
                    if (countRow == occurrence) row = true;
                }
                if (col)
                    for (int i = 0; i < 9; i++)
                    {
                        if (i != xSubgrid && i != xSubgrid + 1 && i != xSubgrid + 2 && Grid[i, y] == 0)
                        {
                            used = true;
                            possible[i][y].Remove(num);
                        }
                    }
                else if (row)
                    for (int i = 0; i < 9; i++)
                    {
                        if (i != ySubgrid && i != ySubgrid + 1 && i != ySubgrid + 2 && Grid[x, i] == 0)
                        {
                            used = true;
                            possible[x][i].Remove(num);
                        }
                    }
            }
            
            return used;
        }
        
       

        /// <summary>
        /// Adds numbers that are possible only in one cell in row, column and subgrid.
        /// </summary>
        /// <param name="x">x index of empty cell</param>
        /// <param name="y">y index of empty cell</param>
        /// <returns>True if any number was added.</returns>
        private bool AlonePossible(int x, int y)
        {
            List<int> nums = possible[x][y];
            int containsRow, containsColumn, containsSubgrid;
            //Debug.Print("\n---------------------------\nX:" + x + " Y:" + y + " NUM:" + num);
            //Debug.Assert(!(x == 5 && y == 8));
            foreach (int num in nums)
            {
                containsRow = 0;
                containsColumn = 0;
                containsSubgrid = 0;
                for (int i = 0; i < 9; i++)
                {
                    int subX = i / 3 + x / 3 * 3;
                    int subY = i % 3 + y / 3 * 3;
                    if (i != y && possible[x][i].Contains(num) )           //ROW
                    {
                        containsRow++;
                    }
                    if (i != x && possible[i][y].Contains(num) )           //COLUMN
                    {
                        containsColumn++;
                    }
                    if ((subX != x || subY != y) && possible[subX][subY].Contains(num))     //SUBGRID
                    {
                        containsSubgrid++;
                    }
                    if (containsRow > 0 && containsColumn > 0 && containsSubgrid > 0) break;
                }
                if (containsRow == 0 || containsColumn == 0 || containsSubgrid == 0)
                {
                    Grid[x, y] = num;
                    possible[x][y] = new List<int> {num};
                    DebugGridPrint("ALONE X: " + x + " Y: " + y + " NUM: " + Grid[x, y]);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes number on indexes from possible in row, column and subgrid.
        /// </summary>
        /// <param name="x">x index of cell with number.</param>
        /// <param name="y">y index of cell with number.param>
        /// <returns>True if any possibility was removed.</returns>
        private bool RemovePossible(int x, int y)
        {
            bool removed = false;
            int num = Grid[x, y];
            //Debug.Print("\n---------------------------\nX:" + x + " Y:" + y + " NUM:" + num);
            //Debug.Assert(!(x == 4 && y == 0));
            for (int i = 0; i < 9; i++)
            {
                int subX = i / 3 + x / 3 * 3;
                int subY = i % 3 + y / 3 * 3;

                if (possible[x][i].Remove(num) == true) removed = true;         //Row
                if (possible[i][y].Remove(num) == true) removed = true;        //Column
                if (possible[subX][subY].Remove(num) == true) removed = true;   //Subgrid
            /*
            Debug.Print("\nROW: " + x + " " + i);
            debugListPrint(possible[x][i]);
            Debug.Print("\nCOL: " + i + " " + y);
            debugListPrint(possible[i][y]);
            Debug.Print("\nSUB: " + subX + " " + subY);
            debugListPrint(possible[subX][subY]);*/
            }
            possible[x][y].Add(num);
            return removed;
        }

        //Helping methods

        /// <summary>
        /// Calculates factorial.
        /// </summary>
        /// <param name="number">Input number</param>
        /// <returns>Result</returns>
        private int Fact(int number)
        {
            int count = 1;
            for (int i = 1; i <= number; i++)
            {
                count *= i;
            }
            return count;
        }

        //count
        /// <summary>
        /// Returns how many times specified number occurs in row.
        /// </summary>
        /// <param name="index">Index of row.</param>
        /// <param name="num">Specified number.</param>
        /// <returns>Integer number of occurrence.</returns>
        private int CountRow(int index, int num)
        {
            int count = 0;
            foreach (List<int> item in GetPossibleRow(index))
            {
                if (item.Contains(num)) count++;
            }
            return count;
        }

        /// <summary>
        /// Returns how many times specified number occurs in column.
        /// </summary>
        /// <param name="index">Index of column.</param>
        /// <param name="num">Specified number.</param>
        /// <returns>Integer number of occurrence.</returns>
        private int CountColumn(int index, int num)
        {
            int count = 0;
            foreach (List<int> item in GetPossibleColumn(index))
            {
                if (item.Contains(num)) count++;
            }
            return count;
        }

        /// <summary>
        /// Returns how many times specified number occurs in subgrid.
        /// </summary>
        /// <param name="x">X index of subgrid.</param>
        /// <param name="y">Y index of subgrid.</param>
        /// <param name="num">Specified number.</param>
        /// <returns>Integer number of occurrence.</returns>
        private int CountSubgrid(int x, int y, int num)
        {
            int count = 0;
            foreach (List<int> item in GetPossibleSubgrid(x, y))
            {
                if (item.Contains(num)) count++;
            }
            return count;
        }

        //missing
        /// <summary>
        /// Returns missing numbers in specified row.
        /// </summary>
        /// <param name="index">Index of row.</param>
        /// <returns>List of integers missing in row.</returns>
        private List<int> MissingNumbersInRow(int index)
        {
            List<int> missing = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            foreach (int number in GetRow(index))
            {
                if (missing.Contains(number)) missing.Remove(number); 
            }
            return missing;
        }

        /// <summary>
        /// Returns missing numbers in specified column.
        /// </summary>
        /// <param name="index">Index of column.</param>
        /// <returns>List of integers missing in column.</returns>
        private List<int> MissingNumbersInColumn(int index)
        {
            List<int> missing = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            foreach (int number in GetColumn(index))
            {
                if (missing.Contains(number)) missing.Remove(number);
            }
            return missing;
        }

        /// <summary>
        /// Returns missing numbers in specified subgrid.
        /// </summary>
        /// <param name="x">Index of row, where subgrid is.</param>
        /// <param name="y">Index of column, where subgrid is.</param>
        /// <returns>List of integers missing in subgrid.</returns>
        private List<int> MissingNumbersInSubgrid(int x, int y)
        {
            List<int> missing = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            foreach (int number in GetSubgrid(x, y))
            {
                if (missing.Contains(number)) missing.Remove(number);
            }
            return missing;
        }

        //possible
        /// <summary>
        /// Returns possible numbers in cells in row.
        /// </summary>
        /// <param name="index">Index of row</param>
        /// <returns>List of Lists of possible numbers in each cell in row.</returns>
        private List<List<int>> GetPossibleRow(int index)
        {
            return possible[index];
        }

        /// <summary>
        /// Returns possible numbers in cells in column.
        /// </summary>
        /// <param name="index">Index of column</param>
        /// <returns>List of Lists of possible numbers in each cell in column.</returns>
        private List<List<int>> GetPossibleColumn(int index)
        {
            List<List<int>> outPossible = new List<List<int>>();
            foreach (List<List<int>> item in possible)
            {
                outPossible.Add(item[index]);
            }
            return outPossible;
        }

        /// <summary>
        /// Returns possible numbers in cells in subgrid.
        /// </summary>
        /// <param name="x">x index of subgrid</param>
        /// <param name="y">y index of subgrid</param>
        /// <returns>List of Lists of possible numbers in each cell in subgrid.</returns>
        private List<List<int>> GetPossibleSubgrid(int x, int y)
        {
            List<List<int>> outPossible = new List<List<int>>();
            int row = x / 3;
            int col = y / 3;
            for (int i = 0; i < 9; i++)
            {
                outPossible.Add(possible[i / 3 + row * 3][i % 3 + col * 3]);
            }
            return outPossible;
        }
    }
}
