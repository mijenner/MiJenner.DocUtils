using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTetris
{
    public enum Command
    {
        MoveLeft,
        MoveRight,
        MoveOneDown,
        MoveDownBoost,
        Drop,
        Rotate,
        Hold,
        None
    }

    internal class TestSourceFile
    {
        // UI: console settings: 
        private int windowHeight;
        private int windowWidth;
        private int boardHeight;
        private int boardWidth;
        private int offLeft;
        private int offTop;
        private GameBoard board;
        private char[,] backBuffer;  // (left, top)
        private ConsoleInput input;
        private ConsoleRenderer renderer;
        private char ch;
        // Game objects, tetromino pieces: 
        private Tetromino gameObject;
        private Tetromino nextObject;
        private Tetromino holdObject;
        private TetrominoFactory factory;
        // Game flow control 
        const int FRAMESTOMOVE = 5; // normal speed 
        const int FASTFRAMESTOMOVE = 2;
        private int fc;  // frame counter; 
        private Command nextCommand;

        public GameController(int windowWidth, int windowHeight, int boardWidth, int boardHeight, int offLeft, int offTop)
        {
            this.windowHeight = windowHeight;
            this.windowWidth = windowWidth;
            this.boardHeight = boardHeight;
            this.boardWidth = boardWidth;
            this.offLeft = offLeft;
            this.offTop = offTop;
            backBuffer = new char[windowWidth, windowHeight];
            ch = 'o';
        }

        internal Tetromino NewPiece()
        {
            Tetromino newPiece = factory.CreateRandomTetromino(board.startR, board.startC);
            return newPiece;
        }

        internal void InitializeGame()
        {
            board = new GameBoard(boardHeight, boardWidth);
            fc = FRAMESTOMOVE;
            input = new ConsoleInput();
            renderer = new ConsoleRenderer(backBuffer);
            factory = new TetrominoFactory();
            gameObject = NewPiece();
            nextObject = NewPiece();
            nextCommand = Command.None;
            BufferClear();
            BufferFrame('*');
            BufferPiece(gameObject, ch);
            renderer.Render();
        }

        internal void BufferClear()
        {
            for (int c = 0; c < windowWidth; c++)
            {
                for (int r = 0; r < windowHeight; r++)
                {
                    backBuffer[c, r] = ' ';
                }
            }
        }

        internal void BufferFrame(char ch)
        {
            // Adds character ch as frame around board. 
            // Upper left: offLeft-1, offTop-1 
            // Lower right: offLeft-1+board.Cols+1, offTop-1+board.Rows+1
            // Top and bottom: 
            for (int c = 0; c < boardWidth + 2; c++)
            {
                backBuffer[offLeft - 1 + c, offTop - 1] = ch;
                backBuffer[offLeft - 1 + c, offTop + boardHeight] = ch;
            }
            // Left and right
            for (int r = 0; r < boardHeight + 1; r++)
            {
                backBuffer[offLeft - 1, offTop - 1 + r] = ch;
                backBuffer[offLeft + boardWidth, offTop - 1 + r] = ch;
            }

        }

        internal void BufferPiece(Tetromino piece, char ch)
        {
            // Note: Tetromino's are stored (row, col) format. 
            // Console uses (left, top) format.
            int left;
            int top;
            for (int r = 0; r < piece.Grid.GetLength(0); r++)
            {
                for (int c = 0; c < piece.Grid.GetLength(1); c++)
                {
                    if (piece.Grid[r, c] == 1)
                    {
                        // offset by -2, -2 so location is 
                        // center-specified. 
                        left = offLeft + piece.C + c - 2;
                        top = offTop + piece.R + r - 2;
                        if (left < windowWidth && top < windowHeight)
                        {
                            backBuffer[left, top] = ch;
                        }
                    }
                }
            }
        }

        internal void Update(float elapsedTime)
        {
            fc--;
            // if fc below 0, move one down 
            if (fc < 0)
            {
                MoveOneDown();
                fc = FRAMESTOMOVE;
            }

            switch (nextCommand)
            {
                case Command.MoveLeft:
                    MoveLeft();
                    break;
                case Command.MoveRight:
                    MoveRight();
                    break;
                case Command.MoveOneDown:
                    break;
                case Command.MoveDownBoost:
                    break;
                case Command.Drop:
                    break;
                case Command.Rotate:
                    break;
                case Command.Hold:
                    break;
                case Command.None:
                    break;
                default:
                    break;
            }

        }

        internal void Render()
        {
            renderer.Render();
        }

        internal void ReadInput()
        {
            nextCommand = input.ReadInput();
        }

        public bool MoveLeft()
        {
            // Check if moving left is possible
            if (board.CanMoveLeft(gameObject))
            {
                BufferPiece(gameObject, ' '); // delete old object 
                gameObject.C--;
                BufferPiece(gameObject, ch); // buffer new object  
                return true;
            }
            return false;
        }

        public bool MoveRight()
        {
            if (board.CanMoveRight(gameObject))
            {
                BufferPiece(gameObject, ' '); // delete old object 
                gameObject.C++;
                BufferPiece(gameObject, ch); // buffer new object  
                return true;
            }
            return false;
        }

        public bool MoveOneDown()
        {
            if (board.CanMoveOneDown(gameObject))
            {
                BufferPiece(gameObject, ' '); // delete old object 
                gameObject.R++;
                BufferPiece(gameObject, ch); // buffer new object  
                return true;
            }
            return false;
        }
    }
}
