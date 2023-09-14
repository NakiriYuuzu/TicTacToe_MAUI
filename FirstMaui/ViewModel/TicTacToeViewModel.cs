using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FirstMaui.model;

namespace FirstMaui.ViewModel;

public partial class TicTacToeViewModel : ObservableObject
{
    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(Player1BackgroundColor))]
    [NotifyPropertyChangedFor(nameof(Player2BackgroundColor))]
    private int _playerTurn;
    public Color Player1BackgroundColor => PlayerTurn == 0 ? Colors.LightBlue : Colors.Black;
    public Color Player2BackgroundColor => PlayerTurn == 1 ? Colors.LightBlue : Colors.Black;
    
    [ObservableProperty] private string _resultText;
    [ObservableProperty] private string _playerText;
    [ObservableProperty] private string _firstPlayerBoard;
    [ObservableProperty] private string _secondPlayerBoard;
    
    private readonly Mode _mode;
    private bool _isGameOver;
    private int _player1Score;
    private int _player2Score;
    public ObservableCollection<GameBoard> BoardsList { get; set; } = new();
    readonly List<int[]> _winningCombinations = new()
    {
        new[] {0, 1, 2},
        new[] {3, 4, 5},
        new[] {6, 7, 8},
        new[] {0, 3, 6},
        new[] {1, 4, 7},
        new[] {2, 5, 8},
        new[] {0, 4, 8},
        new[] {2, 4, 6},
    };

    public TicTacToeViewModel(Mode mode)
    {
        _mode = mode;
        FirstPlayerBoard = $"[X] Player 1: {_player1Score}";
        SecondPlayerBoard = mode == Mode.BOT ? $"[0] Bot: {_player2Score}" : $"[O] Player 2: {_player2Score}";
        SetupOrResetGameBoard();
    }

    [RelayCommand]
    private void SetupOrResetGameBoard()
    {
        _isGameOver = false;
        PlayerTurn = 0;
        ResultText = "";
        PlayerText = PlayerTurn == 0 ? "Player 1 turn" : _mode == Mode.BOT ? "Bot turn" : "Player 2 turn";
        
        BoardsList.Clear();
        for (int i = 0; i < 9; i++)
        {
            BoardsList.Add(new GameBoard(i));
        }
    }

    [RelayCommand]
    private void Click(GameBoard board)
    {
        // Check if the board is already selected
        if (!string.IsNullOrEmpty(board.SelectedType)) return;
        if (_isGameOver) return;

        switch (_mode)
        {
            case Mode.BOT:
                if (PlayerTurn == 0)
                {
                    board.SelectedType = "X";
                    board.Player = 0;
                    CheckForWin();
                    BotMove();
                }
                break;
            case Mode.PLAYER:
                if (PlayerTurn == 0) // Player 1
                {
                    board.SelectedType = "X";
                }
                else if (PlayerTurn == 1) // Player 2
                {
                    board.SelectedType = "O";
                }
                
                board.Player = PlayerTurn;
                
                // Switch player turn
                PlayerTurn = PlayerTurn == 0 ? 1 : 0;
                
                // check for win
                CheckForWin();
                break;
        }
    }

    private void CheckForWin()
    {
        var player1 = BoardsList.Where(x => x.Player == 0).Select(x => x.Index).ToList();
        var player2 = BoardsList.Where(x => x.Player == 1).Select(x => x.Index).ToList();

        // Check if there are enough moves to win
        if (player1.Count < 2 || player2.Count < 2)
        {
            return;
        }

        if (player1.Count + player2.Count == 9)
        {
            GameOver(-1);
            return;
        }
        
        foreach (var combination in _winningCombinations)
        {
            if (combination.All(index => player1.Contains(index)))
            {
                // Player 1 wins
                GameOver(0);
                return;
            }
            if (combination.All(index => player2.Contains(index)))
            {
                // Player 2 or bot wins
                GameOver(1);
                return;
            }
        }
    }
    
    private void GameOver(int winner)
    {
        _isGameOver = true;
        switch (winner)
        {
            case -1:
                ResultText = "Draw!";
                break;
            case 0:
                ResultText = "Player 1 wins!";
                _player1Score += 1;
                FirstPlayerBoard = $"[X] Player 1: {_player1Score}";
                break;
            case 1:
                ResultText = _mode == Mode.BOT ? "Bot Wins!" : "Player 2 wins!";
                _player2Score += 1;
                SecondPlayerBoard = _mode == Mode.BOT ? $"[0] Bot: {_player2Score}" : $"[O] Player 2: {_player2Score}";
                break;
        }
    }

    private void BotMove()
    {
        if (_isGameOver) return;
        
        // Step 1: Check if Bot can win
        foreach (var combination in _winningCombinations)
        {
            var botCells = combination.Where(index => BoardsList[index].Player == 1).ToList();
            if (botCells.Count == 2)
            {
                var emptyCell = combination.Except(botCells).FirstOrDefault();
                if (BoardsList[emptyCell].Player == null)
                {
                    BoardsList[emptyCell].SelectedType = "O";
                    BoardsList[emptyCell].Player = 1;
                    CheckForWin();
                    return;
                }
            }
        }
        
        // Step 2: Block the player from winning
        foreach (var combination in _winningCombinations)
        {
            var playerCells = combination.Where(index => BoardsList[index].Player == 0).ToList();
            if (playerCells.Count == 2)
            {
                var emptyCell = combination.Except(playerCells).FirstOrDefault();
                if (BoardsList[emptyCell].Player == null)
                {
                    BoardsList[emptyCell].SelectedType = "O";
                    BoardsList[emptyCell].Player = 1;
                    CheckForWin();
                    return;
                }
            }
        }
        
        // Step 3: Random move
        Random rand = new Random();
        int move;
        do
        {
            move = rand.Next(0, 9);
        } while (BoardsList[move].Player != null);
        BoardsList[move].SelectedType = "O";
        BoardsList[move].Player = 1;
        
        CheckForWin();
    }
}