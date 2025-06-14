using System;
using System.Windows.Controls;
using SeBattle2.Models;
using System.Windows.Media;

namespace SeBattle2.Logic
{
    internal class AiShipBoardGenerator
    {
        private static readonly Random _rand = new();
        // Ships configuration: [size, count]
        private static readonly (int Size, int Count)[] ShipConfigs = 
        {
            (4, 1),  // 1 ship of size 4
            (3, 2),  // 2 ships of size 3
            (2, 3),  // 3 ships of size 2
            (1, 4)   // 4 ships of size 1
        };

        public Button[,] PlaceShips()
        {
            var board = new Button[10, 10];
            
            // Initialize empty board with buttons and CellInfo
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    board[x, y] = new Button
                    {
                        Tag = new CellInfo 
                        { 
                            Row = x, 
                            Column = y, 
                            HasShip = false 
                        }
                    };
                }
            }

            var shipPlacementChecker = new ShipPlacementChecker(board);

            // Place ships of each size
            foreach (var config in ShipConfigs)
            {
                // Place multiple ships of the same size
                for (int shipCount = 0; shipCount < config.Count; shipCount++)
                {
                    bool placed = false;
                    while (!placed)
                    {
                        // Random coordinates (0-9 instead of 1-10 for array indexing)
                        int x = _rand.Next(0, 10);
                        int y = _rand.Next(0, 10);

                        var result = shipPlacementChecker.Check(x, y, config.Size);

                        if (result.Allowed)
                        {
                            if (config.Size == 1)
                            {
                                // For single unit ships, just place it
                                ((CellInfo)board[x, y].Tag).HasShip = true;
                                placed = true;
                            }
                            else if (result.AllowedDirections.Count > 0)
                            {
                                // Randomly select one of the allowed directions
                                var direction = result.AllowedDirections[_rand.Next(result.AllowedDirections.Count)];
                                PlaceShip(board, x, y, config.Size, direction);
                                placed = true;
                            }
                        }
                    }
                }
            }

            return board;
        }

        private void PlaceShip(Button[,] board, int x, int y, int size, ShipDirection direction)
        {
            for (int i = 0; i < size; i++)
            {
                int currentX = x;
                int currentY = y;

                switch (direction)
                {
                    case ShipDirection.Left:
                        currentX = x - i;
                        break;
                    case ShipDirection.Right:
                        currentX = x + i;
                        break;
                    case ShipDirection.Up:
                        currentY = y - i;
                        break;
                    case ShipDirection.Down:
                        currentY = y + i;
                        break;
                }

                var cellInfo = (CellInfo)board[currentX, currentY].Tag;
                cellInfo.HasShip = true;
            }
        }
    }
}