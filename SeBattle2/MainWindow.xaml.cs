using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SeBattle2.Logic;
using SeBattle2.Models;

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
        private readonly Button[,] _cells = new Button[10, 10];

        private bool _isPlacingShipMode = false;
        private int _placingShipX;
        private int _placingShipY;
        private List<ShipDirection> _placingDirections = new List<ShipDirection>();
        private Button _initialPlacementCell = null; // Add this field at the class level

        private readonly ShipPlacementChecker _shipPlacementChecker;

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

            _shipPlacementChecker = new ShipPlacementChecker(_cells);
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
            int x = cellInfo.Column;
            int y = cellInfo.Row;

            int shipSize = GetCurrentPlacingShipSize();
            var checkResult = _shipPlacementChecker.Check(x, y, shipSize);
            _placingDirections = checkResult.AllowedDirections;

            //1
            if (SmallShipCheckBox.IsChecked == true && checkResult.Allowed)
            {
                PlaceShip(button);
                UpdateLabelAndRadioButton();
            }
            //multi unit ship
            else if (shipSize > 1 && _placingDirections.Any())
            {
                _placingDirections.ForEach(direction => DisplayAllowedDirection(x, y, shipSize, direction));
            }

            if (_placingDirections.Any())
            {
                _isPlacingShipMode = true;
                _placingShipX = x;
                _placingShipY = y;
                _initialPlacementCell = button; // Store the initial cell
            }
        }

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
                    if (_placingDirections.Contains(ShipDirection.Left))
                    {
                        PlaceShipInDirection(ShipDirection.Left);
                    }
                    break;
                case Key.Right:
                    if (_placingDirections.Contains(ShipDirection.Right))
                    {
                        PlaceShipInDirection(ShipDirection.Right);
                    }
                    break;
                case Key.Up:
                    if (_placingDirections.Contains(ShipDirection.Up))
                    {
                        PlaceShipInDirection(ShipDirection.Up);
                    }
                    break;
                case Key.Down:
                    if (_placingDirections.Contains(ShipDirection.Down))
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
                return ExtraLargeSize;
            }

            if (LargeShipCheckBox.IsChecked == true)
            {
                return LargeSize;
            }

            if (MediumShipCheckBox.IsChecked == true)
            {
                return MediumSize;
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

            _placingDirections.ForEach(direction =>
            {
                if (selectedDirection != direction)
                {
                    RemoveArrowsForDirection(direction, currentShipSize);
                }
            });

            // Reset placing directions
            _placingDirections = new List<ShipDirection>();
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

        private bool AreAllShipsPlaced()
        {
            return !SmallShipCheckBox.IsEnabled &&
                   !MediumShipCheckBox.IsEnabled &&
                   !LargeShipCheckBox.IsEnabled &&
                   !ExtraLargeShipCheckBox.IsEnabled;
        }

        private void StartBattle()
        {
            if (AreAllShipsPlaced())
            {
                var battleWindow = new BattleWindow(_cells);
                battleWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Please place all ships before starting the battle!");
            }
        }

        private void StartBattleButton_Click(object sender, RoutedEventArgs e)
        {
            if (AreAllShipsPlaced())
            {
                PlaceShipsForAiPalyer();
            }
            else
            {
                MessageBox.Show("Please place all ships before starting the battle!");
            }
        }

        private void PlaceShipsForAiPalyer()
        {
            var battleWindow = new BattleWindow(_cells);
            battleWindow.Show();
            this.Close();
        }
    }
}
