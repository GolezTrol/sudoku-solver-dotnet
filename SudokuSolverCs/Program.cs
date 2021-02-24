using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;

namespace SudokuSolverCs
{
    public class Cell
    {
        public int Value;
        public bool Original;
        public Cell[] Neighbors = new Cell[20];
    }
    public class Sudoku
    {
        public Cell[] Cells = new Cell[81];
        
        public Sudoku()
        {
            for (var i = 0; i < 81; ++i)
            {
                var cell = new Cell();
                Cells[i] = cell;
            }

            for (var i = 0; i < 81; ++i)
            {
                int x, y;
                x = i % 9;
                y = i / 9;
                int dx;
                int n = 0;
                for (dx = 0; dx < 9; dx++)
                {
                    if (dx!=x) Cells[i].Neighbors[n++] = Cells[i-x+dx];
                }

                int dy;
                for (dy = 0; dy < 9; dy++)
                {
                    if (dy!=y) Cells[i].Neighbors[n++] = Cells[dy*9+x];
                }

                dx = x / 3 * 3;
                dy = y / 3 * 3;
                for (var cx = dx; cx < dx+3; cx++)
                for (var cy = dy; cy < dy+3; cy++)
                {
                    if (cx != x & cy != y)
                    {
                        Cells[i].Neighbors[n++] = Cells[cy*9+cx];
                    }
                }
            }
        }

        public void Load(string sudoku)
        {
            var chars = sudoku.ToCharArray();
            for (var i = 0; i < 81; ++i)
            {
                var cell = Cells[i];
                cell.Value  = chars[i] - '0';
                cell.Original = cell.Value > 0;
            }
        }

        public override string ToString()
        {
            string s = "";
            foreach (var cell in Cells)
            {
                s += cell.Value.ToString();
            }

            return s;
        }
        public bool Solve()
        {
            int i = -1;
            Cell cell;
            bool ok;
            int v;
            int claims;
            do
            {
                //Console.WriteLine($"i = {i}");
                // Get the next cell that can be modified
                do
                {
                    if (i == 80) return true;
                    cell = Cells[++i];
                } while (cell.Original);

                // Find the next number it can contain

                claims = 0;


                ok = false;
                foreach(var neighbor in cell.Neighbors)
                {
                    claims |= 1 << neighbor.Value;
                }
                for (v = cell.Value + 1; v <= 9; v++)
                {
                    // Check if the number is already used in any of the related cells
                    ok = (claims & 1 << v) == 0;
                    
                    // If the number is okay, set it in the cell
                    if (ok)
                    {
                        cell.Value = v;
                        break;
                    }
                }

                // If no number found, backtrack.
                if (!ok)
                {
                    cell.Value = 0;
                    do
                    {
                        if (--i == -1)
                        {
                            return false;
                        }
                    } while (Cells[i].Original);
                    --i;
                }
            } while (true);
        }        
    }
    class Program
    {


        static void Display(Sudoku sudoku)
        {
            for(var i = 0; i < 81;i++)
            {
                var cell = sudoku.Cells[i];
                if (cell.Original) {
                    //Console.ForegroundColor = ConsoleColor.Gray; // Doesn't seem to work
                    Console.Write("\u001b[37m"); // White
                }
                else {
                    Console.Write("\u001b[35m"); // Cyan
                }
                Console.Write($"{cell.Value} ");
                if (i % 9 == 8) Console.WriteLine();
            }
            Console.Write("\u001b[0m"); // Reset
        }
        
        static void Main(string[] args)
        {
            var input = File.ReadAllLines("5.txt");
            /*var input = new string[]{
                "100007090030020008009600500005300900010080002600004000300000010040000007007000300",
                "008034060100080000700010000003000000020500910900000007006003801300000020000900040"
            };*/

            int PuzzleCount = 100;
            var sud = new Sudoku();
            var i = 0;
            long totalms = 0;
            string sudoku;
            for(i = 0; i < PuzzleCount; ++i)
            {
                sudoku = input[i];
                var s = Stopwatch.StartNew();
                sud.Load(sudoku);
                sud.Solve();
                var elapsed = s.ElapsedMilliseconds;
                Display(sud);
                //Console.WriteLine($"input   : {sudoku}");
                //Console.WriteLine($"solution: {Solve(sudoku)}");
                Console.WriteLine($"solved in {elapsed} ms");
                totalms += elapsed;

                Console.WriteLine();
                if (++i == 10) break;

                //Console.WriteLine($"average: {totalms / i} ms");
            }
            Console.WriteLine($"solved {i} puzzles in {totalms} ms");
            Console.WriteLine($"average: {totalms / i} ms");
        }
    }
}