using SeBattle2;
using System.Security.Policy;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;





﻿using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SeBattle2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int MediumSize = 2;
        const int LargeSize = 3;
        const int ExtraLargeSize = 4;
        private Button[,] _cells = new Button[10, 10];

        private bool _isPlacingShipMode = false;
        private int _placingShipX;
        private int _placingShipY;
        private AllowedDirections _placingDirections = AllowedDirections.None;
        private Button _initialPlacementCell = null; // Add this field at the class level

        public MainWindow()
        {
            InitializeComponent();

            this.KeyDown += OnKeyDown;

            for (int row = 1; row <= 10; row++)
            {
                for (int col = 1; col <= 10; col++)
                {
                    Button cellButton = new Button
                    {
                        Name = $"Button_{row}_{col}",
                        Tag = new CellInfo
                        {
                            Row = row - 1,
                            Column = col - 1,
                            HasShip = false
                        }
                    };
                    SetEmptyCellStyle(cellButton);

                    _cells[row - 1, col - 1] = cellButton;

                    // Add event handlers for click and hover
                    cellButton.Click += CellButton_Click;
                    // cellButton.MouseEnter += CellButton_MouseEnter;

                    GameGrid.Children.Add(cellButton);
                    Grid.SetRow(cellButton, row);
                    Grid.SetColumn(cellButton, col);
                }
            }
        }

        private void CellButton_MouseEnter(object sender, MouseEventArgs e)
        {
            var button = sender as Button;
            button.Background = Brushes.LightGray; // Highlight cell on hover
        }

        private bool HasCellShip(int row, int col)
        {
            var cellInfo = _cells[row, col].Tag as CellInfo;
            return cellInfo.HasShip;
        }

        private void CellButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isPlacingShipMode)
            {
                return;
            }

            var button = sender as Button;
            var cellInfo = (CellInfo)button.Tag;

            if (cellInfo.HasShip)
            {
                return; // Do nothing if the cell already has a ship
            }

            int x = cellInfo.Column;
            int y = cellInfo.Row;
            _placingDirections = AllowedDirections.None;

            if (SmallShipCheckBox.IsChecked == true)
            {
                if (IsFreeArea(x - 1, y - 1, x + 1, y + 1))
                {
                    PlaceShip(button);
                    UpdateLabelAndRadioButton();
                }
            }
            else if (MediumShipCheckBox.IsChecked == true)
            {
                //left
                if (IsFreeArea(x - 2, y - 1, x + 1, y + 1))
                {
                    DisplayAllowedDirection(x, y, MediumSize, ShipDirection.Left);
                    _placingDirections = AllowedDirections.Left;
                }

                //right
                if (IsFreeArea(x - 1, y - 1, x + 2, y + 1))
                {
                    DisplayAllowedDirection(x, y, MediumSize, ShipDirection.Right);
                    _placingDirections |= AllowedDirections.Right;
                }

                //up
                if (IsFreeArea(x - 1, y - 2, x + 1, y + 1))
                {
                    DisplayAllowedDirection(x, y, MediumSize, ShipDirection.Up);
                    _placingDirections |= AllowedDirections.Up;
                }

                //down
                if (IsFreeArea(x - 1, y - 1, x + 1, y + 2))
                {
                    DisplayAllowedDirection(x, y, MediumSize, ShipDirection.Down);
                    _placingDirections |= AllowedDirections.Down;
                }
            }
            else if (LargeShipCheckBox.IsChecked == true)
            {
                //left
                if (IsFreeArea(x - 3, y - 1, x + 1, y + 1))
                {
                    DisplayAllowedDirection(x, y, LargeSize, ShipDirection.Left);
                    _placingDirections = AllowedDirections.Left;
                }

                //right
                if (IsFreeArea(x - 1, y - 1, x + 3, y + 1))
                {
                    DisplayAllowedDirection(x, y, LargeSize, ShipDirection.Right);
                    _placingDirections |= AllowedDirections.Right;
                }

                //up
                if (IsFreeArea(x - 1, y - 3, x + 1, y + 1))
                {
                    DisplayAllowedDirection(x, y, LargeSize, ShipDirection.Up);
                    _placingDirections |= AllowedDirections.Up;
                }

                //down
                if (IsFreeArea(x - 1, y - 1, x + 1, y + 3))
                {
                    DisplayAllowedDirection(x, y, LargeSize, ShipDirection.Down);
                    _placingDirections |= AllowedDirections.Down;
                }
            }
            else if (ExtraLargeShipCheckBox.IsChecked == true)
            {
                //left
                if (IsFreeArea(x - 4, y - 1, x + 1, y + 1))
                {
                    DisplayAllowedDirection(x, y, ExtraLargeSize, ShipDirection.Left);
                    _placingDirections = AllowedDirections.Left;
                }

                //right
                if (IsFreeArea(x - 1, y - 1, x + 4, y + 1))
                {
                    DisplayAllowedDirection(x, y, ExtraLargeSize, ShipDirection.Right);
                    _placingDirections |= AllowedDirections.Right;
                }

                //up
                if (IsFreeArea(x - 1, y - 4, x + 1, y + 1))
                {
                    DisplayAllowedDirection(x, y, ExtraLargeSize, ShipDirection.Up);
                    _placingDirections |= AllowedDirections.Up;
                }

                //down
                if (IsFreeArea(x - 1, y - 1, x + 1, y + 4))
                {
                    DisplayAllowedDirection(x, y, ExtraLargeSize, ShipDirection.Down);
                    _placingDirections |= AllowedDirections.Down;
                }
            }

            if (ThereIsAllowedDirection())
            {
                _isPlacingShipMode = true;
                _placingShipX = x;
                _placingShipY = y;
                _initialPlacementCell = button; // Store the initial cell
            }
        }

        private bool ThereIsAllowedDirection()
        {
            return
                _placingDirections.HasFlag(AllowedDirections.Left) ||
                _placingDirections.HasFlag(AllowedDirections.Right) ||
                _placingDirections.HasFlag(AllowedDirections.Up) ||
                _placingDirections.HasFlag(AllowedDirections.Down);
        }

        /*
         * 
         * okeay we knwo in what directions we can plac ship
         * BUT
         * how do we display for user direction to chose
         * 
         * options:
         * 1) dispay kind of arrows 
         *  we display arrows in alloed directions, when user select one of it ship would be placed
         *      how user select arrow:
         *          by mouse click, hard
         *          by navigation arrows, up, down, left, rifght easy, becaus we can easily track what key was pressed by users
         *  2) do not dispaly allowed directions
         *  when user moves moouse to desired direction we either just put ship or just do nothing
         * */

        private void DisplayAllowedDirection(int x, int y, int shipLegth, ShipDirection direction)
        {
            Dictionary<ShipDirection, Uri> arrows =
                new Dictionary<ShipDirection, Uri>
                {
                    { ShipDirection.Down, new Uri("pack://application:,,,/Images/down.png") },
                    { ShipDirection.Up, new Uri("pack://application:,,,/Images/up.png") },
                    { ShipDirection.Left, new Uri("pack://application:,,,/Images/left.png") },
                    { ShipDirection.Right, new Uri("pack://application:,,,/Images/right.png") },
                };

            var arrowImage = new ImageBrush(new BitmapImage(arrows[direction]));

            int deltaX = 0;
            int deltaY = 0;
            switch (direction)
            {
                case ShipDirection.Down:
                    deltaY = 1;
                    break;
                case ShipDirection.Up:
                    deltaY = -1;
                    break;
                case ShipDirection.Left:
                    deltaX = -1;
                    break;
                case ShipDirection.Right:
                    deltaX = 1;
                    break;
            }

            for (int i = 1; i < shipLegth; i++)
            {
                int colIndex = x + deltaX * i;
                int rowIndex = y + deltaY * i;

                Button cell = _cells[rowIndex, colIndex];
                cell.Background = arrowImage;
                cell.BorderBrush = Brushes.LightBlue;
                cell.BorderThickness = new Thickness(5);
            }
        }

        private void UpdateLabelAndRadioButton(Label shipLabel, RadioButton shipCheckBox)
        {
            string shipsLeft = ((string)shipLabel.Content);
            int shipsLeftCount = int.Parse(shipsLeft.Substring(0, 1)) - 1;
            shipLabel.Content = $"{shipsLeftCount} left";
            if (shipsLeftCount == 0)
            {
                shipCheckBox.IsEnabled = false;
                shipCheckBox.IsChecked = false;
            }

            // Reset arrows if no ships are left
            if (shipsLeftCount == 0)
            {
                RemoveArrows(null);
            }
        }

        private void EnableLabelAndRadioButton(int shipsCount, Label shipLabel, RadioButton shipCheckBox)
        {
            shipLabel.Content = $"{shipsCount} left";
            shipCheckBox.IsEnabled = true;
        }

        bool IsFreeArea(int startX, int startY, int endX, int endY)
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

        private static void PlaceShip(Button button)
        {
            ((CellInfo)button.Tag).HasShip = true;
            button.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/hit_icon.png")));
            button.BorderBrush = Brushes.Red;
            button.BorderThickness = new Thickness(5);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!_isPlacingShipMode)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Left:
                    if (_placingDirections.HasFlag(AllowedDirections.Left))
                    {
                        PlaceShipInDirection(ShipDirection.Left);
                    }
                    break;
                case Key.Right:
                    if (_placingDirections.HasFlag(AllowedDirections.Right))
                    {
                        PlaceShipInDirection(ShipDirection.Right);
                    }
                    break;
                case Key.Up:
                    if (_placingDirections.HasFlag(AllowedDirections.Up))
                    {
                        PlaceShipInDirection(ShipDirection.Up);
                    }
                    break;
                case Key.Down:
                    if (_placingDirections.HasFlag(AllowedDirections.Down))
                    {
                        PlaceShipInDirection(ShipDirection.Down);
                    }
                    break;
                case Key.Escape:
                    {
                        ResetShipPlacement();
                        break;
                    }
            }
        }

        private void ResetShipPlacement()
        {
            if (_initialPlacementCell != null)
            {
                // Reset the initial cell's appearance
                SetEmptyCellStyle(_initialPlacementCell);
                ((CellInfo)_initialPlacementCell.Tag).HasShip = false;
            }

            // Remove all arrows
            RemoveArrows(null);

            // Reset placement mode
            _isPlacingShipMode = false;
            _initialPlacementCell = null;
        }

        private void PlaceShipInDirection(ShipDirection direction)
        {
            int shipSize = GetCurrentPlacingShipSize();

            // First, place the ship at the selected position
            PlaceShip(_cells[_placingShipY, _placingShipX]);

            int deltaX = 0;
            int deltaY = 0;

            switch (direction)
            {
                case ShipDirection.Down:
                    deltaY = 1;
                    break;
                case ShipDirection.Up:
                    deltaY = -1;
                    break;
                case ShipDirection.Left:
                    deltaX = -1;
                    break;
                case ShipDirection.Right:
                    deltaX = 1;
                    break;
            }

            // Place the rest of the ship
            for (int i = 1; i < shipSize; i++)
            {
                int colIndex = _placingShipX + deltaX * i;
                int rowIndex = _placingShipY + deltaY * i;

                var button = _cells[rowIndex, colIndex];
                PlaceShip(button);
            }

            // Remove all arrows before updating labels
            RemoveArrows(null);

            // Update labels and reset placing mode
            UpdateLabelAndRadioButton();
            _isPlacingShipMode = false;
            _initialPlacementCell = null; // Reset the initial cell reference
        }

        private int GetCurrentPlacingShipSize()
        {
            if (ExtraLargeShipCheckBox.IsChecked == true)
            {
                return 4;
            }

            if (LargeShipCheckBox.IsChecked == true)
            {
                return 3;
            }

            if (MediumShipCheckBox.IsChecked == true)
            {
                return 2;
            }

            return 1;
        }

       
        private void UpdateLabelAndRadioButton()
        {
            if (ExtraLargeShipCheckBox.IsChecked == true)
            {
                UpdateLabelAndRadioButton(ExtraLargeShipLabel, ExtraLargeShipCheckBox);
            }

            else if (LargeShipCheckBox.IsChecked == true)
            {
                UpdateLabelAndRadioButton(LargeShipLabel, LargeShipCheckBox);
            }

            else if (MediumShipCheckBox.IsChecked == true)
            {
                UpdateLabelAndRadioButton(MediumShipLabel, MediumShipCheckBox);
            }

            else if (SmallShipCheckBox.IsChecked == true)
            {
                UpdateLabelAndRadioButton(SmallShipLabel, SmallShipCheckBox);
            }
        }

        private void RemoveArrows(ShipDirection? selectedDirection)
        {
            // Store current ship size before any radio buttons are changed
            int currentShipSize = GetCurrentPlacingShipSize();

            if (selectedDirection != ShipDirection.Left && _placingDirections.HasFlag(AllowedDirections.Left))
            {
                RemoveArrowsForDirection(ShipDirection.Left, currentShipSize);
            }

            if (selectedDirection != ShipDirection.Right && _placingDirections.HasFlag(AllowedDirections.Right))
            {
                RemoveArrowsForDirection(ShipDirection.Right, currentShipSize);
            }

            if (selectedDirection != ShipDirection.Up && _placingDirections.HasFlag(AllowedDirections.Up))
            {
                RemoveArrowsForDirection(ShipDirection.Up, currentShipSize);
            }

            if (selectedDirection != ShipDirection.Down && _placingDirections.HasFlag(AllowedDirections.Down))
            {
                RemoveArrowsForDirection(ShipDirection.Down, currentShipSize);
            }

            // Reset placing directions
            _placingDirections = AllowedDirections.None;
        }

        private void RemoveArrowsForDirection(ShipDirection direction, int shipSize)
        {
            int deltaX = 0;
            int deltaY = 0;
            switch (direction)
            {
                case ShipDirection.Down:
                    deltaY = 1;
                    break;
                case ShipDirection.Up:
                    deltaY = -1;
                    break;
                case ShipDirection.Left:
                    deltaX = -1;
                    break;
                case ShipDirection.Right:
                    deltaX = 1;
                    break;
            }

            for (int i = 1; i < shipSize; i++)
            {
                int colIndex = _placingShipX + deltaX * i;
                int rowIndex = _placingShipY + deltaY * i;

                if (IsValidCellCoordinate(colIndex) && IsValidCellCoordinate(rowIndex))
                {
                    Button button = _cells[rowIndex, colIndex];
                    if ((button.Tag as CellInfo)?.HasShip == true)
                    {
                        continue; // Skip if the cell already has a ship
                    }

                    SetEmptyCellStyle(button);
                }
            }
        }
        private static void SetEmptyCellStyle(Button button)
        {
            button.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/empty.png")));
            button.BorderBrush = Brushes.Black;
            button.BorderThickness = new Thickness(1);
        }

        private void ClearShipsButton_Click(object sender, RoutedEventArgs e)
        {
            for (int row = 1; row <= 10; row++)
            {
                for (int col = 1; col <= 10; col++)
                {
                    Button button = _cells[row - 1, col - 1];
                    SetEmptyCellStyle(button);
                    (button.Tag as CellInfo).HasShip = false;
                }
            }

            EnableLabelAndRadioButton(1, ExtraLargeShipLabel, ExtraLargeShipCheckBox);
            EnableLabelAndRadioButton(2, LargeShipLabel, LargeShipCheckBox);
            EnableLabelAndRadioButton(3, MediumShipLabel, MediumShipCheckBox);
            EnableLabelAndRadioButton(4, SmallShipLabel, SmallShipCheckBox);
        }

        class CellInfo
        {
            public int Row { get; set; }
            public int Column { get; set; }
            public bool HasShip { get; set; }
        }

        public enum ShipDirection
        {
            Left,
            Down,
            Up,
            Right
        }

        [Flags]
        public enum AllowedDirections
        {
            None = 0,
            Left = 1,
            Down = 2,
            Up = 4,
            Right = 8
        }
    }
}
