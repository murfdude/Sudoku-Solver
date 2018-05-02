using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDD3400_Sudoku
{
    class SudokuBoard
    {
        public int BOARD_SIZE = 9;
        public char BOARD_EMPTY = '_';
        private char[,] board;
        static Queue<SudokuBoard> ToVisit = new Queue<SudokuBoard>();
        static Stack<SudokuBoard> ToVisitStack = new Stack<SudokuBoard>();
        public int choiceCounter;
        public List<char> choices;
        public int moves = 0;



        /// <summary>
        /// SudokuBoard Constructor
        /// </summary>
        public SudokuBoard()
        {
            board = new char[BOARD_SIZE, BOARD_SIZE];

            // For the entire board, initialize each cell
            for (int i = 0; i < BOARD_SIZE; ++i)
            {
                for (int j = 0; j < BOARD_SIZE; ++j)
                {
                    board[i, j] = BOARD_EMPTY;
                }
            }

        }

        /// <summary>
        /// SudokuBoard Copy Constructor
        /// Makes a deep copy of a SudokuBoard
        /// </summary>
        /// <param name="sudoku"></param>
        public SudokuBoard(SudokuBoard sudoku)
        {
            board = new char[BOARD_SIZE, BOARD_SIZE];

            // For the entire board, copy over each cell
            for (int i = 0; i < BOARD_SIZE; ++i)
            {
                for (int j = 0; j < BOARD_SIZE; ++j)
                {
                    board[i, j] = sudoku.board[i, j];
                }
            }


        }

        /// <summary>
        /// Load a Sudoku Board from a file
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool LoadBoardFromFile(string filename)
        {
            // If the file exists
            if (File.Exists(filename))
            {
                string[] text = File.ReadAllLines(filename);

                // Parse the text into sudoku input
                for (int i = 0; i < text.Length; ++i)
                {
                    // If there are not enough lines in the file, fail
                    if (text.Length != BOARD_SIZE)
                    {
                        Console.WriteLine("Improperly formatted file: " + text);
                        return false;
                    }

                    // If there are not enough characters in this row, fail
                    char[] splitters = { ' ' };
                    string[] splitLine = text[i].Split(splitters);
                    if (splitLine.Length != BOARD_SIZE)
                    {
                        Console.WriteLine("Improperly formatted line: " + text[i]);
                        return false;
                    }

                    // For each character in this row, store it in the board
                    for (int j = 0; j < splitLine.Length; ++j)
                    {
                        // Pull off the first character of the string and store it
                        board[i, j] = splitLine[j][0]; 

                        // If the board character is invalid, error
                        if (board[i, j] != BOARD_EMPTY && !(board[i, j] >= '1' && board[i, j] <= (char)('1' + BOARD_SIZE)))
                        {
                            Console.WriteLine("Improperly formatted character: " + board[i, j]);
                            return false;
                        }
                    }
                }
                return true;
            }
            Console.WriteLine("File not found: " + filename);
            return false;
        }

        /// <summary>
        /// Displays the sudoku board
        /// </summary>
        public void Display()
        {
            // For the entire board, initialize each cell
            for (int i = 0; i < BOARD_SIZE; ++i)
            {
                for (int j = 0; j < BOARD_SIZE; ++j)
                {
                    Console.Write(board[i, j] + " ");
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Sets the cell value that corresponds to (row, col) to value
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetCellValue(int row, int col, char value)
        {
            // Only store the cell value if the cell is currently empty
            if (board[row, col] == BOARD_EMPTY)
            {
                board[row, col] = value;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Find all possible values for the given cell that do
        /// not conflict with existing values
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private List<char> PossibleCellValues(int row, int col)
        {
            // If the current cell is already a number, there are no possible values
            if (board[row, col] != BOARD_EMPTY)
            {
                return null;
            }

            List<char> values = new List<char>();

            // Go through all possible values for this cell and see which don't conflict
            for (int i = 0; i < BOARD_SIZE; ++i)
            {
                board[row, col] = (char)('1' + i);
                if (IsRowValid(row) && IsColumnValid(col) && IsBlockValid(row, col))
                {
                    values.Add(board[row, col]);
                }
            }

            board[row, col] = BOARD_EMPTY;

            return values;
        }

        /// <summary>
        /// Determines if the entire board is valid
        /// </summary>
        /// <returns></returns>
        private bool IsBoardValid()
        {
            // Validate each row, each column, and each square
            for (int i = 0; i < BOARD_SIZE; ++i)
            {
                for (int j = 0; j < BOARD_SIZE; ++j)
                {
                    if (!IsRowValid(i) || !IsColumnValid(j) || !IsBlockValid(i, j))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Determines if a 3x3 block is valid and has no duplicates
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        private bool IsBlockValid(int row, int col)
        {
            // Find the block starting row & col
            int blockRow = 3 * (row / 3);
            int blockCol = 3 * (col / 3);

            int iRow;
            int iCol;
            int jRow;
            int jCol;

            // For each cell in the block
            for (int i = 0; i < BOARD_SIZE - 1; ++i)
            {
                iRow = blockRow + i / 3;
                iCol = blockCol + i % 3;

                // For each "other" cell in the block, compare them
                for (int j = i + 1; j < BOARD_SIZE; ++j)
                {
                    jRow = blockRow + j / 3;
                    jCol = blockCol + j % 3;

                    // If there is a duplicate, return the block is not valid
                    if (board[iRow, iCol] != BOARD_EMPTY
                        && board[iRow, iCol] == board[jRow, jCol])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Determines if the column is valid and has no duplicates
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        private bool IsColumnValid(int column)
        {
            for (int i = 0; i < BOARD_SIZE - 1; ++i)
            {
                for (int j = i + 1; j < BOARD_SIZE; ++j)
                {
                    if (board[i, column] != BOARD_EMPTY
                        && board[i, column] == board[j, column])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Determine if the row is valid and has no duplicates
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private bool IsRowValid(int row)
        {
            for (int i = 0; i < BOARD_SIZE - 1; ++i)
            {
                for (int j = i + 1; j < BOARD_SIZE; ++j)
                {
                    if (board[row, i] != BOARD_EMPTY
                        && board[row, i] == board[row, j])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Find the first empty cell in the board, if there are
        /// no empty cells, return (-1, -1)
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private void FindEmptyCell(out int row, out int col)
        {
            for (int i = 0; i < BOARD_SIZE; ++i)
            {
                for (int j = 0; j < BOARD_SIZE; ++j)
                {
                    if (board[i, j] == BOARD_EMPTY)
                    {
                        row = i;
                        col = j;
                        return;
                    }
                }
            }
            row = -1;
            col = -1;
        }

        /// <summary>
        /// Breadth-First Search of Sudoku solutions
        /// Finds the first empty cel and adds all posible options to a Queue (FIFO) And once it finds a solution it knows its the shortest
        /// <returns></returns>
        public SudokuBoard BreadthFirstSearch()
        {
            int row = 0;
            int col = 0;
            List<char> possibleCells;
            SudokuBoard solution = new SudokuBoard(this);
            solution.FindEmptyCell(out row, out col);


            //as long as current board is not true solution keep going
            while (solution.IsBoardValid() && !(row == -1) && !(col == -1)) { 
                //find first empty cell (returning if no empty cells) and all possible values then add those boards to the queue
                solution.FindEmptyCell(out row, out col);
                if (row == -1 && col == -1) 
                {
                    return this;
                }
                possibleCells = solution.PossibleCellValues(row, col);
                foreach (char element in possibleCells)
                {
                    SudokuBoard nextBoard = new SudokuBoard(solution);
                    nextBoard.SetCellValue(row, col, element);
                    ToVisit.Enqueue(nextBoard);
                } 

                    solution = new SudokuBoard(ToVisit.Dequeue());
                    solution.FindEmptyCell(out row, out col);

            }   


            //return if solution found
            solution.FindEmptyCell(out row, out col);
            if (solution.IsBoardValid() && row == -1 && col == -1)
            {
                ToVisit.Clear();
                return solution;
            }


            return null;
        }

        /// <summary>
        /// Depth-First Search of Sudoku solutions
        /// Re implements breadth first with a Stack (FILO) rather than a Queue so if the first choice leads to a solution it goese there
        /// first and doesn't waste time on other partial paths
        /// <returns></returns>
        public SudokuBoard DepthFirstSearch()
        {
            int row = 0;
            int col = 0;
            List<char> possibleCells;
            SudokuBoard solution = new SudokuBoard(this);
            FindEmptyCell(out row, out col);


            //as long as current board is not true solution keep going
            while (solution.IsBoardValid() && !(row == -1) && !(col == -1))
            {
                //find first empty cell (returning if no empty cells) and all possible values then add those boards to the stack
                solution.FindEmptyCell(out row, out col);
                if (row == -1 && col == -1)
                {
                    return this;
                }
                possibleCells = solution.PossibleCellValues(row, col);
                foreach (char element in possibleCells)
                {
                    SudokuBoard nextBoard = new SudokuBoard(solution);
                    nextBoard.SetCellValue(row, col, element);
                    ToVisitStack.Push(nextBoard);
                }

                solution = new SudokuBoard(ToVisitStack.Pop());
                solution.FindEmptyCell(out row, out col);

            }


            //return if solution found
            solution.FindEmptyCell(out row, out col);
            if (solution.IsBoardValid() && row == -1 && col == -1)
            {
                ToVisit.Clear();
                return solution;
            }


            return null;
        }

        /// <summary>
        /// Dijkstra's Search of Sudoku solutions
        /// Uses a Queue such as Breadth first but after each element is added to the queue it is reordered based on number next moves 
        /// so that the best options are tried first
        /// <returns></returns>
        public SudokuBoard BestFirstSearch()
        {
            int row = 0;
            int col = 0;
            int nextRow = 0;
            int nextCol = 0;
            List<char> possibleCells;
            SudokuBoard solution = new SudokuBoard(this);
            solution.FindEmptyCell(out row, out col);


            //as long as current board is not true solution keep going
            while (solution.IsBoardValid() && !(row == -1) && !(col == -1))
            {
                //find first empty cell (returning if no empty cells) and all possible values then add those boards to the queue
                solution.FindEmptyCell(out row, out col);
                if (row == -1 && col == -1)
                {
                    return this;
                }
                possibleCells = solution.PossibleCellValues(row, col);
                foreach (char element in possibleCells)
                {
                    SudokuBoard nextBoard = new SudokuBoard(solution);
                    nextBoard.SetCellValue(row, col, element);
                    nextBoard.moves++;
                    nextBoard.FindEmptyCell(out nextRow, out nextCol);
                    if (nextRow != -1 && nextCol != -1)
                    {
                        nextBoard.choices = nextBoard.PossibleCellValues(nextRow, nextCol);
                        nextBoard.choiceCounter = nextBoard.choices.Count();
                    }
                    ToVisit.Enqueue(nextBoard);                  
                }

                ToVisit.OrderBy(board => board.choiceCounter).ThenBy(board => board.moves);
                solution = new SudokuBoard(ToVisit.Dequeue());
                solution.FindEmptyCell(out row, out col);

            }


            //return if solution found
            solution.FindEmptyCell(out row, out col);
            if (solution.IsBoardValid() && row == -1 && col == -1)
            {
                ToVisit.Clear();
                return solution;
            }


            return null;
        }

        /// <summary>
        /// A-star Search of Sudoku solutions
        /// Improves upon best first by ordering moves based on number of choices from that board as well as progress into the board so as not to waste time on less solved boards
        /// <returns></returns>
        public SudokuBoard AStarSearch()
        {
            int row = 0;
            int col = 0;
            int nextRow = 0;
            int nextCol = 0;
            List<char> possibleCells;
            SudokuBoard solution = new SudokuBoard(this);
            solution.FindEmptyCell(out row, out col);


            //as long as current board is not true solution keep going
            while (solution.IsBoardValid() && !(row == -1) && !(col == -1))
            {
                //find first empty cell (returning if no empty cells) and all possible values then add those boards to the queue
                solution.FindEmptyCell(out row, out col);
                if (row == -1 && col == -1)
                {
                    return this;
                }
                possibleCells = solution.PossibleCellValues(row, col);
                foreach (char element in possibleCells)
                {
                    SudokuBoard nextBoard = new SudokuBoard(solution);
                    nextBoard.SetCellValue(row, col, element);
                    nextBoard.FindEmptyCell(out nextRow, out nextCol);
                    if (nextRow != -1 && nextCol != -1)
                    {
                        nextBoard.choices = nextBoard.PossibleCellValues(nextRow, nextCol);
                        nextBoard.choiceCounter = nextBoard.choices.Count();
                    }
                    ToVisit.Enqueue(nextBoard);
                }

                ToVisit.OrderBy(board => board.choiceCounter);
                solution = new SudokuBoard(ToVisit.Dequeue());
                solution.FindEmptyCell(out row, out col);

            }


            //return if solution found
            solution.FindEmptyCell(out row, out col);
            if (solution.IsBoardValid() && row == -1 && col == -1)
            {
                ToVisit.Clear();
                return solution;
            }
            return null;
        }

    }
}
