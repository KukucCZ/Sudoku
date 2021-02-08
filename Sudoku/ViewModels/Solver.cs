using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace Sudoku.ViewModels
{
    class Solver
    {
        private Validator mainGrid;
        private List<List<List<int>>> possible;
        public Solver(Validator grid)
        {
            mainGrid = grid;
            possible = new List<List<List<int>>>();
            for (int i = 0; i < 9; i++)
            {
                possible.Add(new List<List<int>>());
                for (int j = 0; j < 9; j++)
                {
                    possible[i].Add(new List<int>());
                }
            }
        } 
        
        public int solveAll(bool possibleAlone = true, bool possiblePair = true, int hidden = 0)
        {
            bool changed = false;
            int changedCounter = 0;
            int iteration = 0;
            debugGridPrint();
            //for (int x = 0; x < 9; x++) for (int y = 0; y < 9; y++) if (mainGrid.grid[x, y] != 0) possible[x][y].Add(mainGrid.grid[x, y]);
            for(; ; )
            {
                //Adding numbers to possible.
                for (int x = 0; x < 9; x++) for (int y = 0; y < 9; y++) if (mainGrid.grid[x, y] == 0) intersect(x, y);

                //Removing numbers from possible.
                for (int x = 0; x < 9; x++) for (int y = 0; y < 9; y++) if (mainGrid.grid[x, y] != 0) removePossible(x, y);

                if (possiblePair) for (int x = 0; x < 9; x++) for (int y = 0; y < 9; y++) if (mainGrid.grid[x, y] == 0) removePair(x, y);

                if (hidden != 0) for (int x = 0; x < 9; x++) for (int y = 0; y < 9; y++) if (mainGrid.grid[x, y] == 0)
                            {
                                removeHidden(x, y);
                                if (hidden == 3) removeHidden(x, y, 3);
                                else removeHidden(x, y, 4);
                            }
                
                //Adding numbers to grid.
                for (int x = 0; x < 9; x++) for (int y = 0; y < 9; y++) if (mainGrid.grid[x, y] == 0)
                        {
                            changed = count(x, y);
                            if (changed)
                            {
                                Debug.Print("COUNT X: " + x + " Y: " + y + " NUM: " + mainGrid.grid[x, y]);
                                debugGridPrint();
                            }
                            if (!changed && possibleAlone) changed = alonePossible(x, y);
                        }

                //Ends loop if nothing is added 10 iterations in row.
                if (changed) changedCounter = 0;
                else changedCounter++;
                if (changedCounter >= 10) break;
                changed = false;

                if (mainGrid.isSolved())
                {
                    Debug.Print("Solved"); 
                    break;
                }

                iteration++;
            }
            return iteration;
        }

        //Debug printing methods
        private void debugGridPrint()
        {
            string helper = "";
            for (int x = 0; x < 9; x++)
            {
                helper = "";
                for (int y = 0; y < 9; y++)
                {
                    if (y % 3 == 0) helper += "| ";
                    if (mainGrid.grid[x, y] == 0) helper +=  "  ";
                    else helper += mainGrid.grid[x, y].ToString() + " ";
                }
                if (x % 3 == 0) Debug.Print("------------------------");
                Debug.Print(helper);
            }
            Debug.Print("");
        }

        private void debugListPrint(List<int> list)
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
        private void intersect(int x, int y)
        {
            List<int> row = missingNumbersInRow(x);
            List<int> col = missingNumbersInColumn(y);
            List<int> sub = missingNumbersInSubgrid(x, y);
            IEnumerable<int> helper = row.Intersect(col.Intersect(sub));
            possible[x][y] = helper.ToList();
        }

        public bool removeHidden(int x, int y, int type = 2)
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
                        if (getPossibleRow(x)[i].Contains(combination[r])) help = true;
                    }
                    if (help) row.Add(new List<int> { x, i });

                    help = false;
                    for (int r = 0; r < type; r++)
                    {
                        if (getPossibleColumn(y)[i].Contains(combination[r])) help = true;
                    }
                    if (help) column.Add(new List<int> { i, y });

                    help = false;
                    for (int r = 0; r < type; r++)
                    {
                        if (getPossibleSubgrid(x, y)[i].Contains(combination[r])) help = true;
                    }
                    if (help) subgrid.Add(new List<int> { i / 3 + (x / 3) * 3, i % 3 + (y / 3) * 3 });
                }

                if (row.Count == type && subgrid.Count <= type)
                {
                    foreach (List<int> item in row)
                    {
                        possible[item[0]][item[1]] = combination;
                        Debug.Print("Hidden row " + item[0] + " " + item[1] + " num " + combination[0]+", "+ combination[1]);
                    }
                    return true;
                }
                if (column.Count == type && subgrid.Count <= type)
                {
                    foreach (List<int> item in column)
                    {
                        possible[item[0]][item[1]] = combination;
                        Debug.Print("Hidden col " + item[0] + " " + item[1] + " num " + combination[0] + ", " + combination[1]);
                    }
                    return true;
                }/*
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
        /// Adds number if it is only possible number in cell.
        /// </summary>
        /// <param name="x">x index of empty cell</param>
        /// <param name="y">y index of empty cell</param>
        /// <returns>True if number was added.</returns>
        private bool count(int x, int y)
        {
            if (possible[x][y].Count == 1)
            {
                mainGrid.grid[x, y] = possible[x][y].First();
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
        private bool removePair(int x, int y)
        {
            bool used = false;
            //Debug.Assert(x != 6);
            int xSubgrid = (x / 3) * 3;
            int ySubgrid = (y / 3) * 3;
            foreach (int num in possible[x][y].ToList())
            {
                bool col = false;
                bool row = false;
                int occurrence = countSubgrid(x, y, num);
                if (occurrence == 2 || occurrence == 3)
                {
                    int countCol = 0;
                    int countRow = 0;
                    for (int i = 0; i < 3; i++)
                    {
                        if (mainGrid.grid[xSubgrid + i, y] == 0 && possible[xSubgrid + i][y].Contains(num)) countCol++;
                        if (mainGrid.grid[x, ySubgrid + i] == 0 && possible[x][ySubgrid + i].Contains(num)) countRow++;
                    }
                    if (countCol == occurrence) col = true;
                    if (countRow == occurrence) row = true;
                }
                if (col)
                    for (int i = 0; i < 9; i++)
                    {
                        if (i != xSubgrid && i != xSubgrid + 1 && i != xSubgrid + 2 && mainGrid.grid[i, y] == 0)
                        {
                            used = true;
                            possible[i][y].Remove(num);
                        }
                    }
                else if (row)
                    for (int i = 0; i < 9; i++)
                    {
                        if (i != ySubgrid && i != ySubgrid + 1 && i != ySubgrid + 2 && mainGrid.grid[x, i] == 0)
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
        private bool alonePossible(int x, int y)
        {
            List<int> nums = possible[x][y];
            int containsRow, containsColumn, containsSubgrid;
            //Debug.Print("\n---------------------------\nX:" + x + " Y:" + y + " NUM:" + num);
            //Debug.Assert(!(x == 1 && y == 6));
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
                    mainGrid.grid[x, y] = num;
                    possible[x][y] = new List<int> {num};
                    Debug.Print("ALONE X: " + x + " Y: " + y + " NUM: " + mainGrid.grid[x, y]);
                    debugGridPrint();
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
        private bool removePossible(int x, int y)
        {
            bool removed = false;
            int num = mainGrid.grid[x, y];
            //Debug.Print("\n---------------------------\nX:" + x + " Y:" + y + " NUM:" + num);
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
        private int fact(int number)
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
        private int countRow(int index, int num)
        {
            int count = 0;
            foreach (List<int> item in getPossibleRow(index))
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
        private int countColumn(int index, int num)
        {
            int count = 0;
            foreach (List<int> item in getPossibleColumn(index))
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
        private int countSubgrid(int x, int y, int num)
        {
            int count = 0;
            foreach (List<int> item in getPossibleSubgrid(x, y))
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
        private List<int> missingNumbersInRow(int index)
        {
            List<int> missing = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            foreach (int number in mainGrid.getRow(index))
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
        private List<int> missingNumbersInColumn(int index)
        {
            List<int> missing = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            foreach (int number in mainGrid.getColumn(index))
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
        private List<int> missingNumbersInSubgrid(int x, int y)
        {
            List<int> missing = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            foreach (int number in mainGrid.getSubgrid(x, y))
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
        private List<List<int>> getPossibleRow(int index)
        {
            return possible[index];
        }

        /// <summary>
        /// Returns possible numbers in cells in column.
        /// </summary>
        /// <param name="index">Index of column</param>
        /// <returns>List of Lists of possible numbers in each cell in column.</returns>
        private List<List<int>> getPossibleColumn(int index)
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
        private List<List<int>> getPossibleSubgrid(int x, int y)
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
