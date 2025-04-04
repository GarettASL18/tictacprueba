using Microsoft.Maui.Graphics.Text;

namespace TicTacToe
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        private int player1Wins = 0;
        private int player2Wins = 0;
        private bool isPlayerOneTurn = true;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnButtonClicked(object sender, EventArgs e)
        {
            var button = sender as Button;

            if (button != null && button.Text == null)
            {
                button.Text = isPlayerOneTurn ? "X" : "O";
                button.Background = new SolidColorBrush(isPlayerOneTurn ? Colors.Blue : Colors.Green);
                isPlayerOneTurn = !isPlayerOneTurn;
                button.IsEnabled = false;
                CheckForWinner();
            }

            if (CheckForWinner())
            {
                if (!isPlayerOneTurn) // Check who the winner is
                {
                    player1Wins++;
                    Player1WinsLabel.Text = $"Jugador 1 gana : {player1Wins}";
                }
                else
                {
                    player2Wins++;
                    Player2WinsLabel.Text = $"Jugador 2 gana : {player2Wins}";
                }

                await DisplayAlert("Fin del juego", $"Jugador {(isPlayerOneTurn ? 2 : 1)} Gana!", "OK");
                ResetGame();
            }
            else if (IsDraw())
            {
                await DisplayAlert("Fin del juego", "Han empatado!", "OK");
                ResetGame();
            }
        }

        private bool CheckForWinner()
        {
            string[,] grid = new string[3, 3];

            // Fill the grid with the current state
            grid[0, 0] = Button0.Text;
            grid[0, 1] = Button1.Text;
            grid[0, 2] = Button2.Text;
            grid[1, 0] = Button3.Text;
            grid[1, 1] = Button4.Text;
            grid[1, 2] = Button5.Text;
            grid[2, 0] = Button6.Text;
            grid[2, 1] = Button7.Text;
            grid[2, 2] = Button8.Text;

            // Check rows
            for (int i = 0; i < 3; i++)
            {
                if (!string.IsNullOrEmpty(grid[i, 0]) &&
                    grid[i, 0] == grid[i, 1] && grid[i, 1] == grid[i, 2])
                    return true;
            }

            // Check columns
            for (int i = 0; i < 3; i++)
            {
                if (!string.IsNullOrEmpty(grid[0, i]) &&
                    grid[0, i] == grid[1, i] && grid[1, i] == grid[2, i])
                    return true;
            }

            // Check diagonals
            if (!string.IsNullOrEmpty(grid[0, 0]) &&
                grid[0, 0] == grid[1, 1] && grid[1, 1] == grid[2, 2])
                return true;

            if (!string.IsNullOrEmpty(grid[0, 2]) &&
                grid[0, 2] == grid[1, 1] && grid[1, 1] == grid[2, 0])
                return true;

            return false;
        }

        private bool IsDraw()
        {
            return Button0.Text != null && Button1.Text != null && Button2.Text != null && Button3.Text != null
                && Button4.Text != null && Button5.Text != null && Button6.Text != null && Button7.Text != null
                && Button8.Text != null;
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
    }
}
