using System;
using System.Linq;
using System.Collections.Generic;
using static TicTacToeGame.TicTacToeBoard;

namespace TicTacToeGame
{
    public class TicTacToeBoard
    {
        public enum GameStatus
        {
            Win = 0,
            CanPlay = 1,
            Finsih = 2,
            Invalid = -1
        }

        public enum TwoPlayers
        {
            Player1 = 'A',
            Player2 = 'B'
        }
        public class Move
        {
            public Move(ushort row, ushort col)
            {
                Row = row;
                Col = col;
            }
            public ushort Row { get; set; }
            public ushort Col { get; set; }
        }

        private const short BoardSize = 3;
        private char[,] board = new char[BoardSize, BoardSize];
        private List<Move> winMoves = new List<Move>();
        private Dictionary<GameStatus, ConsoleColor> colorMaps = new Dictionary<GameStatus, ConsoleColor>
        {
            {GameStatus.CanPlay, ConsoleColor.Cyan },
            {GameStatus.Win, ConsoleColor.DarkGreen },
            {GameStatus.Invalid, ConsoleColor.Red },
            {GameStatus.Finsih, ConsoleColor.DarkYellow },
        };
        private GameStatus lastGameStatus = GameStatus.CanPlay;
        private TwoPlayers lastPlayedPlayer = TwoPlayers.Player2;

        private void PrintMove(ushort i, ushort j, string append = "")
        {
            const string format = " {0} ";
            var curChar = board[i, j] == '\0' ? '-' : board[i, j];
            string opText = string.Format(format, curChar);
            if (winMoves.FirstOrDefault(move => move.Row == i && move.Col == j) != null)
            {
                PrintColorOutput(opText, ConsoleColor.DarkGreen, false);
            }
            else
            {
                Console.Write(opText);
            }

            Console.Write(append);
        }

        public void PrintBoard()
        {
            for (ushort i = 0; i < BoardSize; ++i)
            {
                for (ushort j = 0; j < BoardSize; ++j)
                {
                    if (j == BoardSize - 1)
                        PrintMove(i, j);
                    else
                        PrintMove(i, j, "|");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public void Reset()
        {
            board = new char[BoardSize, BoardSize];
            winMoves.Clear();
            lastGameStatus = GameStatus.CanPlay;
            lastPlayedPlayer = TwoPlayers.Player2;
            PrintColorOutput("\n\nRestarting the game.\n\n", ConsoleColor.DarkMagenta);
        }

        public GameStatus Play(TwoPlayers player, Move move)
        {
            string errorMessage;
            var isValidMove = IsValidMove(player, move, out errorMessage);
            GameStatus boardStatus;
            if (isValidMove)
            {
                boardStatus = GetBoardStatus(player, move);
                lastGameStatus = boardStatus;
                lastPlayedPlayer = player;
            }
            else
            {
                PrintColorOutput(errorMessage, ConsoleColor.Red, true);
                boardStatus = GameStatus.Invalid;
            }
            var moveMessage = $"Player{(char)player}({move.Row},{move.Col}) made {Enum.GetName(boardStatus.GetType(), boardStatus)} move.\n";
            PrintColorOutput(moveMessage, colorMaps[boardStatus]);
            PrintBoard();
            return boardStatus;
        }

        private void PrintColorOutput(string message, ConsoleColor color, bool newLine = true)
        {
            Console.ForegroundColor = color;
            if (newLine)
            {
                Console.WriteLine(message);
            }
            else
            {
                Console.Write(message);
            }
            Console.ResetColor();
        }

        private GameStatus GetBoardStatus(TwoPlayers player, Move move)
        {
            bool moveAvailable = false;
            char p = (char)player;

            //Mark move
            board[move.Row, move.Col] = p;

            //First diagonal checks
            if (IsWinMove(p, new Move(0, 0), new Move(1, 1), new Move(2, 2)))
            {
                return GameStatus.Win;
            }

            //Second diagonal checks
            if (IsWinMove(p, new Move(0, 2), new Move(1, 1), new Move(2, 0)))
            {
                return GameStatus.Win;
            }

            for (ushort i = 0; i < BoardSize; ++i)
            {
                //Row check.
                if (IsWinMove(p, new Move(0, i), new Move(1, i), new Move(2, i)))
                {
                    return GameStatus.Win;
                }
                //Column check.
                if (IsWinMove(p, new Move(i, 0), new Move(i, 1), new Move(i, 2)))
                {
                    return GameStatus.Win;
                }

                //To decide board is available for next move.
                if (!moveAvailable && (board[0, i] == '\0' || board[1, i] == '\0' || board[2, i] == '\0'))
                {
                    moveAvailable = true;
                }
            }

            if (!moveAvailable)
                return GameStatus.Finsih;
            else
                return GameStatus.CanPlay;
        }

        private bool IsWinMove(char lVal, params Move[] moves)
        {
            var isWin = moves.All(curMove => board[curMove.Row, curMove.Col] == lVal);
            if (isWin)
            {
                winMoves.AddRange(moves);
            }
            return isWin;
        }

        private bool IsValidMove(TwoPlayers player, Move move, out string errorMessage)
        {
            bool result = false;

            if (lastGameStatus == GameStatus.Finsih || lastGameStatus == GameStatus.Win)
            {
                errorMessage = "Game over, cannot make this move.";
            }
            else if (player == lastPlayedPlayer)
            {
                errorMessage = $"Player{(char)player} cannot make consucative moves.";
            }
            else if (move.Row >= BoardSize)
            {
                errorMessage = $"Game row '{move.Row}' cannot exceed max board size '{BoardSize - 1}'.";
            }
            else if (move.Col >= BoardSize)
            {
                errorMessage = $"Game column '{move.Col}' cannot exceed max board size '{BoardSize - 1}'.";
            }
            else if (board[move.Row, move.Col] != '\0')
            {
                errorMessage = $"Move ({move.Row},{move.Col}) already made by Player{board[move.Row, move.Col]}.";
            }
            else
            {
                result = true;
                errorMessage = "Validation success!";
            }

            return result;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var sb = new TicTacToeBoard();
            GameStatus res = sb.Play(TwoPlayers.Player1, new Move(0, 0));
            res = sb.Play(TwoPlayers.Player2, new Move(2, 0));
            res = sb.Play(TwoPlayers.Player1, new Move(0, 1));
            res = sb.Play(TwoPlayers.Player2, new Move(2, 1));
            res = sb.Play(TwoPlayers.Player1, new Move(2, 2));
            res = sb.Play(TwoPlayers.Player2, new Move(0, 2));
            res = sb.Play(TwoPlayers.Player1, new Move(1, 0));
            res = sb.Play(TwoPlayers.Player2, new Move(1, 1));
            // Post win move.
            res = sb.Play(TwoPlayers.Player1, new Move(1, 2));

            sb.Reset();

            res = sb.Play(TwoPlayers.Player1, new Move(0, 0));
            res = sb.Play(TwoPlayers.Player2, new Move(1, 1));
            res = sb.Play(TwoPlayers.Player1, new Move(0, 1));
            res = sb.Play(TwoPlayers.Player2, new Move(0, 2));
            res = sb.Play(TwoPlayers.Player1, new Move(2, 0));
            res = sb.Play(TwoPlayers.Player2, new Move(1, 0));
            res = sb.Play(TwoPlayers.Player1, new Move(1, 2));
            res = sb.Play(TwoPlayers.Player2, new Move(2, 1));
            res = sb.Play(TwoPlayers.Player1, new Move(2, 2));

            // Post finish move.
            res = sb.Play(TwoPlayers.Player1, new Move(2, 2));


            Console.ReadKey();
            sb.Reset();
        }
    }
}
