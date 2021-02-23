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
        public List<Cell> Neighbors = new List<Cell>();
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
                for (dx = 0; dx < 9; dx++)
                {
                    if (dx!=x) Cells[i].Neighbors.Add(Cells[i-x+dx]);
                }

                int dy;
                for (dy = 0; dy < 9; dy++)
                {
                    if (dy!=y) Cells[i].Neighbors.Add(Cells[dy*9+x]);
                }

                dx = x / 3 * 3;
                dy = y / 3 * 3;
                for (var cx = 0; cx < 3; cx++)
                for (var cy = 0; cy < 3; cy++)
                {
                    if (dx + cx != x || dy + cy != y)
                    {
                        int di = (dy + cy) * 9 + dx + cx;
                        if (!Cells[i].Neighbors.Contains(Cells[di]))
                            Cells[i].Neighbors.Add(Cells[di]);
                    }
                    if (dy!=y) Cells[i].Neighbors.Add(Cells[dy*9+x]);
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
    }
    class Program
    {
        static string Solve(Sudoku sudoku)
        {
            int i = -1;
            Cell cell;
            do
            {
                //Console.WriteLine($"i = {i}");
                // Get the next cell that can be modified
                do
                {
                    if (i == 80) return sudoku.ToString() + " yoo 1";
                    cell = sudoku.Cells[++i];
                } while (cell.Original);

                // Find the next number it can contain
                bool ok = false;
                for (int v = cell.Value + 1; v <= 9; v++)
                {
                    // Check if the number is already used in any of the related cells
                    ok = true;
                    foreach(var n in cell.Neighbors)
                    {
                        if (n.Value == v)
                        {
                            ok = false;
                            break;
                        }
                    }
                    
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
                            return "NOOOOOOOO solution";
                        }

                        cell = sudoku.Cells[i];
                        if (!cell.Original)
                        {
                            if (cell.Value == 9)
                            {
                                cell.Value = 0;
                            }
                            else
                            {
                                i--;
                                break;
                            }
                        }
                    } while (true);
                }
            } while (true);

            return "No solution found";

        }

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
            var sud = new Sudoku();
            var i = 0;
            long totalms = 0;
            foreach(var sudoku in input)
            {
                sud.Load(sudoku);

                var s = Stopwatch.StartNew();
                Solve(sud);
                Display(sud);
                //Console.WriteLine($"input   : {sudoku}");
                //Console.WriteLine($"solution: {Solve(sudoku)}");
                Console.WriteLine($"solved in {s.ElapsedMilliseconds} ms");
                totalms += s.ElapsedMilliseconds;

                Console.WriteLine();
                if (++i == 10) break;

                //Console.WriteLine($"average: {totalms / i} ms");
            }
            Console.WriteLine($"solved {i} puzzles in {totalms} ms");
            Console.WriteLine($"average: {totalms / i} ms");
        }
    }
}