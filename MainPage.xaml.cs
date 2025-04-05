using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToe
{
    public partial class MainPage : ContentPage
    {
        int player1Wins = 0;
        int player2Wins = 0;
        bool isPlayerOneTurn = true;
        enum GameMode { PvP, PvAI }
        GameMode currentGameMode = GameMode.PvP;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnButtonClicked(object sender, EventArgs e)
        {
            var button = sender as Button;

            if (button != null && string.IsNullOrEmpty(button.Text))
            {
                button.Text = isPlayerOneTurn ? "X" : "O";
                button.Background = new SolidColorBrush(isPlayerOneTurn ? Colors.Blue : Colors.Green);
                button.IsEnabled = false;

                if (CheckForWinner())
                {
                    if (isPlayerOneTurn)
                    {
                        player1Wins++;
                        Player1WinsLabel.Text = $"Victorias del jugador 1: {player1Wins}";
                    }
                    else
                    {
                        player2Wins++;
                        Player2WinsLabel.Text = $"Victorias del jugador 2: {player2Wins}";
                    }

                    await DisplayAlert("Fin del juego", $"Jugador {(isPlayerOneTurn ? 1 : 2)} gana!", "OK");
                    ResetGame();
                    return;
                }
                else if (IsDraw())
                {
                    await DisplayAlert("Fin del juego", "Empate!", "OK");
                    ResetGame();
                    return;
                }

                isPlayerOneTurn = !isPlayerOneTurn;

                if (currentGameMode == GameMode.PvAI && !isPlayerOneTurn)
                {
                    await MakeAIMove();
                }
            }
        }

        private async Task MakeAIMove()
        {
            await Task.Delay(1000); // Retraso de 1.5 segundos para simular "pensamiento"
            int bestMove = GetBestMove();

            if (bestMove != -1)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Button aiButton = GetButton(bestMove);
                    aiButton.Text = "O";
                    aiButton.Background = new SolidColorBrush(Colors.Green);
                    aiButton.IsEnabled = false;

                    if (CheckForWinner())
                    {
                        player2Wins++;
                        Player2WinsLabel.Text = $"Victorias del jugador 2: {player2Wins}";
                        DisplayAlert("Fin del juego", "La máquina gana!", "OK");
                        ResetGame();
                        return;
                    }
                    else if (IsDraw())
                    {
                        DisplayAlert("Fin del juego", "Empate!", "OK");
                        ResetGame();
                        return;
                    }

                    isPlayerOneTurn = true;
                });
            }
        }

        private int GetBestMove()
        {
            string[,] board = GetBoard();
            int bestScore = int.MinValue;
            int move = -1;

            for (int i = 0; i < 9; i++)
            {
                int row = i / 3;
                int col = i % 3;

                if (string.IsNullOrEmpty(board[row, col]))
                {
                    board[row, col] = "O"; // Máquina simula su jugada
                    int score = Minimax(board, 0, false);
                    board[row, col] = "";

                    if (score > bestScore)
                    {
                        bestScore = score;
                        move = i;
                    }
                }
            }

            return move;
        }

        private int Minimax(string[,] board, int depth, bool isMaximizing)
        {
            string result = CheckWinner(board);
            if (result != null)
            {
                if (result == "O") return 10 - depth;
                if (result == "X") return depth - 10;
                if (result == "draw") return 0;
            }

            if (isMaximizing)
            {
                int bestScore = int.MinValue;

                for (int i = 0; i < 9; i++)
                {
                    int row = i / 3;
                    int col = i % 3;

                    if (string.IsNullOrEmpty(board[row, col]))
                    {
                        board[row, col] = "O";
                        int score = Minimax(board, depth + 1, false);
                        board[row, col] = "";
                        bestScore = Math.Max(score, bestScore);
                    }
                }

                return bestScore;
            }
            else
            {
                int bestScore = int.MaxValue;

                for (int i = 0; i < 9; i++)
                {
                    int row = i / 3;
                    int col = i % 3;

                    if (string.IsNullOrEmpty(board[row, col]))
                    {
                        board[row, col] = "X";
                        int score = Minimax(board, depth + 1, true);
                        board[row, col] = "";
                        bestScore = Math.Min(score, bestScore);
                    }
                }

                return bestScore;
            }
        }

        private string CheckWinner(string[,] board)
        {
            // Check rows and columns
            for (int i = 0; i < 3; i++)
            {
                if (!string.IsNullOrEmpty(board[i, 0]) &&
                    board[i, 0] == board[i, 1] && board[i, 1] == board[i, 2])
                    return board[i, 0];

                if (!string.IsNullOrEmpty(board[0, i]) &&
                    board[0, i] == board[1, i] && board[1, i] == board[2, i])
                    return board[0, i];
            }

            // Diagonals
            if (!string.IsNullOrEmpty(board[0, 0]) &&
                board[0, 0] == board[1, 1] && board[1, 1] == board[2, 2])
                return board[0, 0];

            if (!string.IsNullOrEmpty(board[0, 2]) &&
                board[0, 2] == board[1, 1] && board[1, 1] == board[2, 0])
                return board[0, 2];

            // Draw
            bool draw = true;
            foreach (var cell in board)
            {
                if (string.IsNullOrEmpty(cell))
                    draw = false;
            }

            return draw ? "draw" : null;
        }

        private string[,] GetBoard()
        {
            return new string[3, 3]
            {
                { Button0.Text ?? "", Button1.Text ?? "", Button2.Text ?? "" },
                { Button3.Text ?? "", Button4.Text ?? "", Button5.Text ?? "" },
                { Button6.Text ?? "", Button7.Text ?? "", Button8.Text ?? "" }
            };
        }

        private Button GetButton(int index)
        {
            return index switch
            {
                0 => Button0,
                1 => Button1,
                2 => Button2,
                3 => Button3,
                4 => Button4,
                5 => Button5,
                6 => Button6,
                7 => Button7,
                8 => Button8,
                _ => throw new ArgumentOutOfRangeException(nameof(index))
            };
        }

        private bool CheckForWinner()
        {
            string result = CheckWinner(GetBoard());
            return result == "X" || result == "O";
        }

        private bool IsDraw()
        {
            return !CheckForWinner() && GetBoard().Cast<string>().All(cell => !string.IsNullOrEmpty(cell));
        }

        private void ResetGame()
        {
            Button[] buttons = { Button0, Button1, Button2, Button3, Button4, Button5, Button6, Button7, Button8 };

            foreach (Button button in buttons)
            {
                button.Text = null;
                button.Background = null;
                button.IsEnabled = true;
            }

            isPlayerOneTurn = true;
        }

        private void OnResetTallyClicked(object sender, EventArgs e)
        {
            player1Wins = 0;
            player2Wins = 0;
            Player1WinsLabel.Text = "Victorias del jugador 1: 0";
            Player2WinsLabel.Text = "Victorias del jugador 2: 0";
        }

        private void OnPvPClicked(object sender, EventArgs e)
        {
            currentGameMode = GameMode.PvP;
            ResetGame();
        }

        private void OnPvAIClicked(object sender, EventArgs e)
        {
            currentGameMode = GameMode.PvAI;
            ResetGame();
        }
    }
}
