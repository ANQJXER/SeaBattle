using System.Dynamic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SeBattle2
{
    public class CellInfo
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public bool HasShip { get; set; }
    }
    public partial class BattleWindow : Window
    {
        private Button[,] _playerCells = new Button[10, 10];
        private Button[,] _enemyCells = new Button[10, 10];

        public BattleWindow(Button[,] playerShips)
        {
            InitializeComponent();
            InitializeGrids();
            CopyPlayerShips(playerShips);
        }

        private void InitializeGrids()
        {
            // Initialize both grids
            InitializeGrid(PlayerGrid, _playerCells, false);
            InitializeGrid(EnemyGrid, _enemyCells, true);
        }

        private void InitializeGrid(Grid grid, Button[,] cells, bool isEnemyGrid)
        {
            // Create columns and rows
            for (int i = 0; i <= 10; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            }

            // Add column headers (A-J)
            for (int col = 1; col <= 10; col++)
            {
                var header = new TextBlock
                {
                    Text = ((char)(64 + col)).ToString(),
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(header, 0);
                Grid.SetColumn(header, col);
                grid.Children.Add(header);
            }

            // Add row headers (1-10)
            for (int row = 1; row <= 10; row++)
            {
                var header = new TextBlock
                {
                    Text = row.ToString(),
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetRow(header, row);
                Grid.SetColumn(header, 0);
                grid.Children.Add(header);
            }

            // Create the buttons for the grid
            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    Button cell = new Button
                    {
                        Tag = new CellInfo
                        {
                            Row = row,
                            Column = col,
                            HasShip = false
                        }
                    };

                    if (isEnemyGrid)
                    {
                        cell.Click += EnemyCell_Click;
                    }

                    SetEmptyCellStyle(cell);

                    cells[row, col] = cell;
                    Grid.SetRow(cell, row + 1);
                    Grid.SetColumn(cell, col + 1);
                    grid.Children.Add(cell);
                }
            }
        }

        private void CopyPlayerShips(Button[,] playerShips)
        {
            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    var sourceCell = playerShips[row, col];
                    var targetCell = _playerCells[row, col];
                    var sourceCellInfo = sourceCell.Tag as CellInfo;

                    if (sourceCellInfo != null && sourceCellInfo.HasShip)
                    {
                        ((CellInfo)targetCell.Tag).HasShip = true;
                        targetCell.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/hit_icon.png")));
                    }
                    if (sourceCellInfo.HasShip)
                    {
                        ((CellInfo)targetCell.Tag).HasShip = true;
                        targetCell.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/hit_icon.png")));
                    }
                }
            }
        }

        private void EnemyCell_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            // Here you would implement the shooting logic
            // For now, we'll just mark it as a miss
            button.Background = Brushes.Blue;
        }

        private static void SetEmptyCellStyle(Button button)
        {
            button.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/empty.png")));
            button.BorderBrush = Brushes.Black;
            button.BorderThickness = new Thickness(1);
        }
    }
}
