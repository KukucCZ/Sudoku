using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace Sudoku.ViewModels
{
    class Solver
    {
        public Validator mainGrid;
        public List<List<List<int>>> possible;
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
        
        public int solve(bool possibleCount = true, bool possibleAlone = true)
        {
            bool changed = false;
            int changedCounter = 0;
            int iteration = 0;
            for(; ; )
            {               
                for (int x = 0; x < 9; x++)
                {
                    for (int y = 0; y < 9; y++)
                    {
                        //Adds intersecting numbers into possible List.
                        if (iteration % 3 == 0 && mainGrid.grid[x, y] == 0)
                        {
                            List<int> row = missingNumbersInRow(x);
                            List<int> col = missingNumbersInColumn(y);
                            List<int> sub = missingNumbersInSubgrid(x, y);
                            IEnumerable<int> helper = row.Intersect(col.Intersect(sub));
                            possible[x][y] = helper.ToList();
                        }
                        //Removes unwanted possibilities from possible List.
                        if (iteration % 3 == 1 && mainGrid.grid[x, y] != 0)
                        {
                            removePossible(x, y);
                        }
                        //If in cell could be only one number, that its added into mainGrid.
                        if (iteration % 3 == 2 && mainGrid.grid[x, y] == 0)
                        { 
                            if (possibleCount && possible[x][y].Count == 1)
                            {
                                changed = true;
                                mainGrid.grid[x, y] = possible[x][y].First();
                                Debug.Print("COUNT X: " + x + " Y: " + y + " NUM: " + mainGrid.grid[x, y]);
                                debugGridPrint();
                            }
                            else if (possibleAlone) changed = alonePossible(x, y);
                        }
                        
                    }
                } 
                //Ends loop if nothing is added 10 iterations in row.
                if (iteration % 3 == 2)
                {
                    if (changed) changedCounter = 0;
                    else changedCounter++;
                }        
                if (changedCounter >= 10) break;
                changed = false;

                if (mainGrid.isSolved()) break;

                iteration++;
            }
            return (iteration + 1 - changedCounter * 3)/3;
        }

        public void debugGridPrint()
        {
            string helper = "";
            for (int x = 0; x < 9; x++)
            {
                helper = "";
                for (int y = 0; y < 9; y++)
                {
                    if (y % 3 == 0) helper += "| ";
                    helper += mainGrid.grid[x, y].ToString() + " ";
                }
                if (x % 3 == 0) Debug.Print("------------------------");
                Debug.Print(helper);
            }
            Debug.Print("");
        }

        public void debugListPrint(List<int> list)
        {
            string helper = "{";
            foreach (int item in list)
            {
                helper += item + ", ";
            }
            Debug.Print(helper + "}");
        }

        /// <summary>
        /// Adds numbers that are possible only in one cell in row, column and subgrid.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool alonePossible(int x, int y)
        {
            List<int> nums = possible[x][y];
            int containsRow, containsColumn, containsSubgrid;
            //Debug.Print("\n---------------------------\nX:" + x + " Y:" + y + " NUM:" + num);
            //Debug.Assert(x != 0);
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
        /// Removes number on indexes from possible.
        /// </summary>
        /// <param name="x">X index of searched number.</param>
        /// <param name="y">X index of searched number.param>
        public void removePossible(int x, int y)
        {
            int num = mainGrid.grid[x, y];
            //Debug.Print("\n---------------------------\nX:" + x + " Y:" + y + " NUM:" + num);
            for (int i = 0; i < 9; i++)
            {
                int subX = i / 3 + x / 3 * 3;
                int subY = i % 3 + y / 3 * 3;
                
                possible[x][i].Remove(num);         //Row
                possible[i][y].Remove(num);         //Column
                possible[subX][subY].Remove(num);   //Subgrid
                /*
                Debug.Print("\nROW: " + x + " " + i);
                debugListPrint(possible[x][i]);
                Debug.Print("\nCOL: " + i + " " + y);
                debugListPrint(possible[i][y]);
                Debug.Print("\nSUB: " + subX + " " + subY);
                debugListPrint(possible[subX][subY]);*/
            }
        }

        /// <summary>
        /// Returns missing numbers in specified row.
        /// </summary>
        /// <param name="index">Index of row.</param>
        /// <returns>List of integers missing in row.</returns>
        public List<int> missingNumbersInRow(int index)
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
        public List<int> missingNumbersInColumn(int index)
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
        public List<int> missingNumbersInSubgrid(int x, int y)
        {
            List<int> missing = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            foreach (int number in mainGrid.getSubgrid(x, y))
            {
                if (missing.Contains(number)) missing.Remove(number);
            }
            return missing;
        }

        public List<List<int>> getPossibleRow(int index)
        {
            return possible[index];
        }

        public List<List<int>> getPossibleColumn(int index)
        {
            List<List<int>> outPossible = new List<List<int>>();
            foreach (List<List<int>> item in possible)
            {
                outPossible.Add(item[index]);
            }
            return outPossible;
        }

        public List<List<int>> getPossibleSubgrid(int x, int y)
        {
            List<List<int>> outPossible = new List<List<int>>();
            int row = x / 3;
            int col = y / 3;
            for (int i = 0; i < 9; i++)
            {
                outPossible[i] = possible[i / 3 + row * 3][i % 3 + col * 3];
            }
            return outPossible;
        }
    }
}
