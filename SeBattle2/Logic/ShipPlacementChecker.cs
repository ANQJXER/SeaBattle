using SeBattle2.Models;
using System.Windows.Controls;

namespace SeBattle2.Logic
{
    public class ShipPlacementChecker
    {
        private readonly Button[,] _board;

        public ShipPlacementChecker(Button[,] board)
        {
            _board = board;
        }

        public ShipPlacementCheckResult Check(int x, int y, int shipSize)
        {
            if (HasCellAShipUnit(x, y))
            {
                return new ShipPlacementCheckResult(false, new List<ShipDirection>());
            }

            if (shipSize == 1)
            {
                return CheckSingleUnitShipPlacement(x, y);
            }

            return CheckMultipleUnitShipPlacement(x, y, shipSize);
        }

        private bool HasCellAShipUnit(int x, int y)
        {
            var cellInfo = (CellInfo)_board[x, y].Tag;
            return cellInfo.HasShip;
        }

        private ShipPlacementCheckResult CheckMultipleUnitShipPlacement(int x, int y, int shipSize)
        {
            var allowedDirections = new List<ShipDirection>();

            //left
            if (IsFreeArea(x - shipSize, y - 1, x + 1, y + 1))
            {
                allowedDirections.Add(ShipDirection.Left);
            }

            //right
            if (IsFreeArea(x - 1, y - 1, x + shipSize, y + 1))
            {
                allowedDirections.Add(ShipDirection.Right);
            }

            //up
            if (IsFreeArea(x - 1, y - shipSize, x + 1, y + 1))
            {
                allowedDirections.Add(ShipDirection.Up);
            }

            //down
            if (IsFreeArea(x - 1, y - 1, x + 1, y + shipSize))
            {
                allowedDirections.Add(ShipDirection.Down);
            }

            if (!allowedDirections.Any())
            {
                return new ShipPlacementCheckResult(false, new List<ShipDirection>());
            }

            return new ShipPlacementCheckResult(true, allowedDirections);
        }

        private ShipPlacementCheckResult CheckSingleUnitShipPlacement(int x, int y)
        {
            if (IsFreeArea(x - 1, y - 1, x + 1, y + 1))
            {
                return new ShipPlacementCheckResult(true, new List<ShipDirection>());
            }

            return new ShipPlacementCheckResult(false, new List<ShipDirection>());
        }

        private bool IsFreeArea(int startX, int startY, int endX, int endY)
        {
            //actual ship location
            for (int x = startX + 1; x <= endX - 1; x++)
            {
                for (int y = startY + 1; y <= endY - 1; y++)
                {
                    int rowIndex = y;
                    int colIndex = x;

                    if (!IsValidCellCoordinate(x)
                        || !IsValidCellCoordinate(y)
                        || HasCellShip(rowIndex, colIndex))
                    {
                        return false;
                    }
                }
            }

            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    int rowIndex = y;
                    int colIndex = x;

                    if (IsValidCellCoordinate(x)
                        && IsValidCellCoordinate(y)
                        && HasCellShip(rowIndex, colIndex))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        bool IsValidCellCoordinate(int coordinate)
        {
            return coordinate >= 0 && coordinate <= 9;
        }

        private bool HasCellShip(int row, int col)
        {
            var cellInfo = _board[row, col].Tag as CellInfo;
            return cellInfo.HasShip;
        }
    }
}
