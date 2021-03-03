using System;
using System.Collections.Generic;
using System.Text;

namespace Sudoku.Models
{
    /// <summary>
    /// Include grid, returns specified row, column and subgrid of grid.
    /// Checks if grid is valid and solved.
    /// </summary>
    class Validator
    {
        public CellsGrid grid;
        public Validator(CellsGrid inputGrid)
        {
            grid = inputGrid;
        }

        /// <summary>
        /// Returns row of grid.
        /// </summary>
        /// <param name="getRow">Index of row</param>
        /// <returns>Array of integers in selected row.</returns>
        public int[] GetRow(int num)
        {
            int[] row = new int[9];
            for (int i = 0; i < 9; i++)
            {
                row[i] = grid[num, i];
            }

            return row;
        }

        /// <summary>
        /// Returns column of grid.
        /// </summary>
        /// <param name="num">Index of column</param>
        /// <returns>Array of integers in selected column.</returns>
        public int[] GetColumn(int num)
        {
            int[] col = new int[9];
            for (int i = 0; i < 9; i++)
            {
                col[i] = grid[i, num];
            }

            return col;
        }

        /// <summary>
        /// Returns subgrid (3 by 3 grid) of grid.
        /// </summary>
        /// <param name="num">Index of subgrid. Counted from left to right, top to bottom (middle left => 3).</param>
        /// <returns>Array of integers in selected subgrid.</returns>
        public int[] GetSubgrid(int num)
        {
            int[] sub = new int[9];
            int row = num / 3;
            int col = num % 3;
            for (int i = 0; i < 9; i++)
            {
                sub[i] = grid[i/3 + row * 3, i%3 + col * 3];
            }

            return sub;
        }

        /// <summary>
        /// Returns subgrid (3 by 3 grid) of grid.
        /// </summary>
        /// <param name="x">Index of row, where subgrid is.</param>
        /// <param name="y">Index of column, where subgrid is.</param>
        /// <returns>Array of integers in selected subgrid.</returns>
        public int[] GetSubgrid(int x, int y)
        {
            int[] sub = new int[9];
            int row = x / 3;
            int col = y / 3;
            for (int i = 0; i < 9; i++)
            {
                sub[i] = grid[i / 3 + row * 3, i % 3 + col * 3];
            }

            return sub;
        }

        /// <summary>
        /// Checks if grid doesn't have errors (same numbers in one row, column or subgrid).
        /// </summary>
        /// <returns>If there are no errors, returns true. If error is found returns false.</returns>
        public bool IsValid()
        {
            int[] row, col, sub;
            string rowTest, colTest, subTest;
            for (int i = 0; i < 9; i++)
            {
                row = GetRow(i);
                col = GetColumn(i);
                sub = GetSubgrid(i);
                rowTest = "";
                colTest = "";
                subTest = "";
                for (int j = 0; j < 9; j++)
                {
                    if ((row[j] != 0 && rowTest.Contains(row[j].ToString())) || (col[j] != 0 && colTest.Contains(col[j].ToString())) || (sub[j] != 0 && subTest.Contains(sub[j].ToString()))) return false;
                    rowTest += row[j].ToString();
                    colTest += col[j].ToString();
                    subTest += sub[j].ToString();
                }
            }
            return true;
        }

        /// <summary>
        /// Checks if grid is solved (no errors and no empty spaces).
        /// </summary>
        /// <returns>If grid isValid and doesn't have 0 (0 => empty) returns true, else returns false.</returns>
        public bool IsSolved()
        {
            if (!IsValid()) return false;
            int[] row;
            for (int i = 0; i < 9; i++)
            {
                row = GetRow(i);
                for (int j = 0; j < 9; j++)
                {
                    if (row[j] == 0) return false;
                }
            }
            return true;
        }
    }
}
