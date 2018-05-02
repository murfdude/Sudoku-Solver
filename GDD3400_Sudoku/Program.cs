using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDD3400_Sudoku
{
    class Program
    {
        public delegate SudokuBoard SearchAlgorithm();

        static void Main(string[] args)
        {
            char menuChoice;
            SudokuBoard sudoku = null;
            SearchAlgorithm search = null;

            do
            {
                Console.WriteLine("**************************************");
                Console.WriteLine("Please select from the following menu:");
                Console.WriteLine("1 - Load a Sudoku File");
                Console.WriteLine("2 - Use Breadth-First to Solve the Puzzle");
                Console.WriteLine("3 - Use Depth-First to Solve the Puzzle");
                Console.WriteLine("4 - Use Best-First to Solve the Puzzle");
                Console.WriteLine("5 - Use A* to Solve the Puzzle");
                Console.WriteLine("6 - Run all Solutions");
                Console.WriteLine("7 - Quit");
                Console.WriteLine("**************************************");

                menuChoice = Console.ReadKey().KeyChar;
                Console.ReadLine();

                switch (menuChoice)
                {
                    case '1':
                        sudoku = LoadFile();
                        break;
                    case '2':
                        if (sudoku != null)
                        {
                            search = new SearchAlgorithm(sudoku.BreadthFirstSearch);
                            RunSearch("Using Breadth-First...", search);
                        }
                        else 
                        {
                            Console.WriteLine("No sudoku board found, load one first.");
                        }
                        break;
                    case '3':
                        if (sudoku != null)
                        {
                            search = new SearchAlgorithm(sudoku.DepthFirstSearch);
                            RunSearch("Using Depth-First...", search);
                        }
                        else 
                        {
                            Console.WriteLine("No sudoku board found, load one first.");
                        }
                        break;
                    case '4':
                        if (sudoku != null)
                        {
                            search = new SearchAlgorithm(sudoku.BestFirstSearch);
                            RunSearch("Using Best-First...", search);
                        }
                        else 
                        {
                            Console.WriteLine("No sudoku board found, load one first.");
                        }
                        break;
                    case '5':
                        if (sudoku != null)
                        {
                            search = new SearchAlgorithm(sudoku.AStarSearch);
                            RunSearch("Using A*...", search);
                        }
                        else 
                        {
                            Console.WriteLine("No sudoku board found, load one first.");
                        }
                        break;
                    case '6':
                        if (sudoku != null)
                        {
                            Console.WriteLine("Running all...");
                            search = new SearchAlgorithm(sudoku.BreadthFirstSearch);
                            RunSearch("Using Breadth-First...", search);
                            search = new SearchAlgorithm(sudoku.DepthFirstSearch);
                            RunSearch("Using Depth-First...", search);
                            search = new SearchAlgorithm(sudoku.BestFirstSearch);
                            RunSearch("Using Best-First...", search);
                            search = new SearchAlgorithm(sudoku.AStarSearch);
                            RunSearch("Using A*...", search);
                        }
                        else 
                        {
                            Console.WriteLine("No sudoku board found, load one first.");
                        }
                        break;
                    case '7':
                        Console.WriteLine("Quitting...");
                        break;
                    default:
                        Console.WriteLine("Please select a valid menu choice");
                        break;
                }
            } while (menuChoice != '7');

            Console.WriteLine("Thanks for using the Sudoku Solver");
            Console.ReadKey();
        }

        /// <summary>
        /// Load the sudoku file into the board
        /// </summary>
        /// <returns></returns>
        public static SudokuBoard LoadFile()
        {
            Console.WriteLine("Load a Sudoku File...");

            Console.WriteLine("Please enter the name and path of the file");
            string filename = Console.ReadLine();

            SudokuBoard sudoku = new SudokuBoard();

            // If the file is unable to be loaded, display an error, return null
            if (!sudoku.LoadBoardFromFile(filename))
            {
                Console.WriteLine("Unable to load from file: " + filename);
                return null;
            }

            sudoku.Display();

            return sudoku;
        }
        /// <summary>
        /// Run the provided search algorithm
        /// </summary>
        /// <param name="message"></param>
        /// <param name="search"></param>
        public static void RunSearch(string message, SearchAlgorithm search)
        {
            Console.WriteLine(message);

            // Time the search
            DateTime start = DateTime.Now;
            SudokuBoard sudoku = search();
            DateTime end = DateTime.Now;

            TimeSpan diff = end - start;

            if (sudoku != null)
            {
                Console.WriteLine("Solution found in " + diff.Hours + "h:" + diff.Minutes + "m:" + diff.Seconds + "s:" + diff.Milliseconds + "ms");
                sudoku.Display();
            }
            else
            {
                Console.WriteLine("No Solution Returned");
            }
        }

    }
}
