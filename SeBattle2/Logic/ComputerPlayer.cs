using System;
using System.Collections.Generic;
using System.Windows.Controls;
using SeBattle2.Models;

namespace SeBattle2.Logic
{
    public class ComputerPlayer
    {
        private readonly Button[,] _playerCells;
        private readonly Random _random;
        private readonly List<(int x, int y)> _potentialTargets;
        private (int x, int y)? _lastHit;

        public event EventHandler<GameEventArgs> OnEnemyShot;

        public ComputerPlayer(Button[,] playerCells)
        {
            _playerCells = playerCells;
            _random = new Random();
            _potentialTargets = new List<(int x, int y)>();
            _lastHit = null;
        }

        public void TakeTurn()
        {
            do
            {
                (int x, int y) target;

                if (_potentialTargets.Count > 0)
                {
                    // Try cells around last hit
                    int index = _random.Next(_potentialTargets.Count);
                    target = _potentialTargets[index];
                    _potentialTargets.RemoveAt(index);
                }
                else
                {
                    // Random shot
                    target = GetRandomTarget();
                }

                ProcessShot(target.x, target.y);
            } 
            while (_lastHit != null);
        }

        private void ProcessShot(int x, int y)
        {
            var cellInfo = (CellInfo)_playerCells[x, y].Tag;

            if (cellInfo.HasShip)
            {
                // Hit
                _lastHit = (x, y);
                AddAdjacentCells(x, y);
            }
            else
            {
                // Miss
                _lastHit = null;
            }

            // Notify the game about the shot
            OnEnemyShot?.Invoke(this, new GameEventArgs { X = x, Y = y });
        }

        private void AddAdjacentCells(int x, int y)
        {
            // Add adjacent cells to potential targets
            var adjacent = new[]
            {
                (x - 1, y), // left
                (x + 1, y), // right
                (x, y - 1), // up
                (x, y + 1)  // down
            };

            foreach (var (adjX, adjY) in adjacent)
            {
                if (IsValidCell(adjX, adjY) && IsCellAvailable(adjX, adjY))
                {
                    _potentialTargets.Add((adjX, adjY));
                }
            }
        }

        private (int x, int y) GetRandomTarget()
        {
            int x, y;
            do
            {
                x = _random.Next(0, 10);
                y = _random.Next(0, 10);
            } while (!IsCellAvailable(x, y));

            return (x, y);
        }

        private bool IsValidCell(int x, int y)
        {
            return x >= 0 && x < 10 && y >= 0 && y < 10;
        }

        private bool IsCellAvailable(int x, int y)
        {
            return _playerCells[x, y].IsEnabled;
        }
    }
}