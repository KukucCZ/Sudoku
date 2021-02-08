﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Sudoku.Models
{
    class Grid
    {
        //public int[,] grid = new int[9, 9];
        public int[,] grid
        {
            get;
            set;
        }

        public Grid()
        {
            /*
            grid = new int[,] { { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },

                                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },

                                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                { 0, 0, 0, 0, 0, 0, 0, 0, 0 } };
                                */

            //SOLVED EASY
            /*
            grid = new int[,] { { 1, 2, 3, 7, 8, 9, 4, 5, 6 },
                                { 4, 5, 6, 1, 2, 3, 7, 8, 9 },
                                { 7, 8, 9, 4, 5, 6, 1, 2, 3 },
                                { 3, 1, 2, 9, 7, 8, 6, 4, 5 },
                                { 6, 4, 5, 3, 1, 2, 9, 7, 8 },
                                { 9, 7, 8, 6, 4, 5, 3, 1, 2 },
                                { 2, 3, 1, 8, 9, 7, 5, 6, 4 },
                                { 5, 6, 4, 2, 3, 1, 8, 9, 7 },
                                { 8, 9, 7, 5, 6, 4, 2, 3, 1 }};
            */
            //EASY
            /*
            grid = new int[,] { { 1, 0, 3, 7, 8, 9, 4, 5, 6 },
                                { 4, 0, 6, 1, 0, 3, 7, 8, 9 },
                                { 7, 8, 9, 4, 5, 6, 1, 0, 3 },

                                { 3, 1, 2, 9, 7, 8, 6, 4, 5 },
                                { 6, 4, 5, 3, 1, 2, 9, 7, 8 },
                                { 9, 7, 8, 6, 4, 5, 3, 1, 2 },

                                { 2, 3, 1, 8, 9, 7, 0, 6, 4 },
                                { 5, 6, 4, 2, 3, 1, 8, 9, 7 },
                                { 8, 9, 7, 5, 6, 4, 2, 3, 1 }};
                                */
            //---------------------------------------------------------------------
            //noviny
            /*
            grid = new int[,] { { 0, 8, 0, 0, 5, 4, 0, 0, 6 },
                                { 0, 2, 0, 0, 0, 7, 5, 0, 0 },
                                { 3, 5, 0, 0, 0, 0, 0, 0, 7 },

                                { 0, 0, 0, 9, 1, 2, 4, 0, 8 },
                                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                { 9, 0, 2, 4, 8, 6, 0, 0, 0 },

                                { 2, 0, 0, 0, 0, 0, 0, 6, 5 },
                                { 0, 0, 7, 3, 0, 0, 0, 1, 0 },
                                { 5, 0, 0, 6, 4, 0, 0, 7, 0 } };
            */
            //casopis
            /*
            grid = new int[,] { { 5, 0, 7, 1, 0, 0, 0, 0, 0 },
                                { 0, 0, 0, 0, 9, 8, 0, 0, 0 },
                                { 0, 8, 1, 7, 0, 0, 0, 2, 0 },

                                { 0, 5, 0, 0, 8, 0, 0, 0, 0 },
                                { 0, 2, 8, 0, 0, 0, 3, 5, 0 },
                                { 0, 0, 0, 0, 4, 0, 0, 1, 0 },

                                { 0, 3, 0, 0, 0, 7, 2, 8, 0 },
                                { 0, 0, 0, 2, 1, 0, 0, 0, 0 },
                                { 0, 0, 0, 0, 0, 4, 7, 0, 3 } };
            */
            //websudoku - HARD  https://www.websudoku.com/?level=3&set_id=7730983785
            /*
            grid = new int[,] { { 0, 0, 0, 0, 0, 0, 0, 7, 0 },
                                { 0, 9, 0, 8, 0, 0, 6, 0, 5 },
                                { 4, 0, 0, 0, 2, 6, 0, 0, 1 },

                                { 0, 0, 0, 3, 0, 0, 0, 8, 0 },
                                { 0, 6, 9, 0, 0, 0, 4, 1, 0 },
                                { 0, 3, 0, 0, 0, 1, 0, 0, 0 },

                                { 2, 0, 0, 7, 3, 0, 0, 0, 6 },
                                { 6, 0, 7, 0, 0, 5, 0, 9, 0 },
                                { 0, 4, 0, 0, 0, 0, 0, 0, 0 } };
                    */
            //HARD https://www.websudoku.com/?level=3&set_id=6927913026
            /*
            grid = new int[,] { { 0, 0, 1, 0, 7, 0, 0, 0, 0 },
                                { 7, 0, 0, 0, 0, 0, 5, 0, 0 },
                                { 5, 0, 0, 3, 0, 6, 0, 7, 0 },

                                { 0, 0, 5, 2, 8, 0, 4, 9, 0 },
                                { 0, 7, 0, 0, 0, 0, 0, 1, 0 },
                                { 0, 4, 8, 0, 3, 9, 6, 0, 0 },

                                { 0, 3, 0, 7, 0, 4, 0, 0, 5 },
                                { 0, 0, 6, 0, 0, 0, 0, 0, 8 },
                                { 0, 0, 0, 0, 1, 0, 3, 0, 0 } };
             */
            /*
           grid = new int[,] { { 3, 0, 0, 1, 4, 0, 0, 0, 0 },
                               { 0, 6, 0, 0, 0, 0, 0, 0, 3 },
                               { 9, 0, 0, 0, 0, 7, 6, 0, 8 },

                               { 0, 9, 7, 0, 8, 0, 1, 0, 0 },
                               { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                               { 0, 0, 3, 0, 5, 0, 2, 9, 0 },

                               { 7, 0, 4, 3, 0, 0, 0, 0, 2 },
                               { 6, 0, 0, 0, 0, 0, 0, 5, 0 },
                               { 0, 0, 0, 0, 7, 5, 0, 0, 4 } };
                               */



            //websudoku - EVIL https://www.websudoku.com/?level=4&set_id=6927913026 - not solved
            /*
            grid = new int[,] { { 0, 0, 5, 3, 0, 0, 0, 0, 7 },
                                { 9, 0, 0, 0, 0, 7, 0, 0, 0 },
                                { 0, 0, 0, 0, 8, 0, 0, 9, 0 },

                                { 0, 0, 7, 1, 0, 0, 4, 0, 9 },
                                { 0, 0, 8, 0, 7, 0, 2, 0, 0 },
                                { 5, 0, 4, 0, 0, 3, 7, 0, 0 },

                                { 0, 3, 0, 0, 9, 0, 0, 0, 0 },
                                { 0, 0, 0, 7, 0, 0, 0, 0, 8 },
                                { 2, 0, 0, 0, 0, 4, 5, 0, 0 } };*/
            //https://www.websudoku.com/?level=4&set_id=626852913
            /*
            grid = new int[,] { { 1, 8, 3, 0, 0, 0, 0, 0, 0 },
                                { 0, 0, 7, 0, 3, 1, 0, 0, 0 },
                                { 0, 0, 0, 9, 0, 0, 0, 7, 0 },

                                { 2, 0, 0, 0, 0, 7, 0, 5, 0 },
                                { 0, 0, 0, 4, 2, 3, 0, 0, 0 },
                                { 0, 4, 0, 1, 0, 0, 0, 0, 6 },

                                { 0, 1, 0, 0, 0, 4, 0, 0, 0 },
                                { 0, 0, 0, 2, 8, 0, 7, 0, 0 },
                                { 0, 0, 0, 0, 0, 0, 6, 2, 4 } };*/
            /*
            //Pair test
            grid = new int[,] { { 4, 6, 5, 0, 8, 0, 3, 2, 0 },
                                { 7, 9, 8, 0, 6, 2, 6, 0, 5 },
                                { 1, 2, 3, 5, 6, 0, 0, 9, 8 },

                                { 8, 0, 0, 2, 0, 5, 0, 3, 0 },
                                { 0, 0, 2, 0, 0, 0, 5, 0, 0 },
                                { 5, 0, 0, 3, 0, 6, 2, 8, 0 },

                                { 0, 8, 4, 0, 5, 3, 1, 7, 2 },
                                { 0, 0, 0, 0, 2, 0, 8, 5, 4 },
                                { 2, 5, 7, 0, 1, 0, 9, 6, 3 } };*/
            //hidden test
            /*
            grid = new int[,] { { 1, 8, 3, 0, 0, 0, 0, 0, 0 },
                                { 0, 0, 7, 0, 3, 1, 0, 0, 0 },
                                { 0, 0, 0, 9, 0, 0, 0, 7, 0 },

                                { 2, 3, 0, 0, 0, 7, 4, 5, 0 },
                                { 0, 0, 0, 4, 2, 3, 0, 0, 7 },
                                { 7, 4, 0, 1, 0, 0, 2, 3, 6 },

                                { 0, 1, 2, 0, 0, 4, 0, 0, 0 },
                                { 0, 0, 0, 2, 8, 0, 7, 0, 0 },
                                { 0, 7, 0, 0, 1, 0, 6, 2, 4 } }; */

            
            grid = new int[,] { { 0, 0, 9, 1, 0, 0, 0, 3, 0 },
                                { 0, 0, 0, 9, 5, 0, 0, 0, 0 },
                                { 8, 2, 0, 4, 0, 0, 0, 0, 0 },

                                { 0, 4, 0, 0, 0, 0, 0, 0, 7 },
                                { 9, 7, 0, 8, 0, 4, 0, 1, 6 },
                                { 3, 0, 0, 0, 0, 0, 0, 9, 0 },

                                { 0, 0, 0, 0, 0, 1, 0, 6, 3 },
                                { 0, 0, 0, 0, 2, 6, 0, 0, 0 },
                                { 0, 9, 0, 0, 0, 0, 5, 0, 0 } };
                                
        }
    }
}
