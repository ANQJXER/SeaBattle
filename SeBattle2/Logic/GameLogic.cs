using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SeBattle2.Logic;
using SeBattle2.Models;
using CellInfo = SeBattle2.CellInfo;

public class GameLogic
{
    private readonly Button[,] _enemyCells;
    private readonly Button[,] _playerCells;
    private readonly ComputerPlayer _computerPlayer;
    private bool _isGameOver;
    private bool _isPlayerTurn;

    public event EventHandler<GameEventArgs> OnHit;
    public event EventHandler<GameEventArgs> OnMiss;
    public event EventHandler OnGameOver;
    public event EventHandler OnComputerTurn;

    public GameLogic(Button[,] enemyCells, Button[,] playerCells)
    {
        _enemyCells = enemyCells;
        _playerCells = playerCells;
        _computerPlayer = new ComputerPlayer(playerCells);
        _isGameOver = false;
        _isPlayerTurn = true;

        // Subscribe to computer's shots
        _computerPlayer.OnEnemyShot += ComputerPlayer_OnEnemyShot;
    }

    public void ProcessShot(int x, int y)
    {
        if (_isGameOver || !_isPlayerTurn) return;

        var cellInfo = (CellInfo)_enemyCells[x, y].Tag;
        
        if (!_enemyCells[x, y].IsEnabled) return;

        if (cellInfo.HasShip)
        {
            // Hit
            _enemyCells[x, y].Background = new ImageBrush(new BitmapImage(
                new Uri("pack://application:,,,/Images/hit_icon.png")));
            OnHit?.Invoke(this, new GameEventArgs { X = x, Y = y });
            
            if (CheckForGameOver(_enemyCells))
            {
                _isGameOver = true;
                OnGameOver?.Invoke(this, EventArgs.Empty);
                return;
            }
        }
        else
        {
            // Miss
            _enemyCells[x, y].Background = new ImageBrush(new BitmapImage(
                new Uri("pack://application:,,,/Images/missed_icon.png")));
            OnMiss?.Invoke(this, new GameEventArgs { X = x, Y = y });
        }

        _enemyCells[x, y].IsEnabled = false;
        
        // Switch to computer's turn
        _isPlayerTurn = false;
        OnComputerTurn?.Invoke(this, EventArgs.Empty);
        
        // Let computer take its turn
        _computerPlayer.TakeTurn();
    }

    private void ComputerPlayer_OnEnemyShot(object sender, GameEventArgs e)
    {
        var cellInfo = (CellInfo)_playerCells[e.X, e.Y].Tag;

        if (cellInfo.HasShip)
        {
            _playerCells[e.X, e.Y].Background = new ImageBrush(new BitmapImage(
                new Uri("pack://application:,,,/Images/hit_icon.png")));

            if (CheckForGameOver(_playerCells))
            {
                _isGameOver = true;
                OnGameOver?.Invoke(this, EventArgs.Empty);
                return;
            }
        }
        else
        {
            _playerCells[e.X, e.Y].Background = new ImageBrush(new BitmapImage(
                new Uri("pack://application:,,,/Images/missed_icon.png")));
        }

        _playerCells[e.X, e.Y].IsEnabled = false;
        _isPlayerTurn = true; // Switch back to player's turn
    }

    private bool CheckForGameOver(Button[,] cells)
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                var cellInfo = (CellInfo)cells[i, j].Tag;
                if (cellInfo.HasShip && cells[i, j].IsEnabled)
                {
                    return false;
                }
            }
        }
        return true;
    }
}

public class GameEventArgs : EventArgs
{
    public int X { get; set; }
    public int Y { get; set; }
}