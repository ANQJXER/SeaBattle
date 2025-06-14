using SeBattle2.Logic;
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
        private readonly Button[,] _playerCells;
        private readonly Button[,] _enemyCells;
        private readonly GameLogic _gameLogic;

        public BattleWindow(Button[,] playerCells)
        {
            InitializeComponent();
            _playerCells = playerCells;
            
            // Initialize enemy board
            var aiGenerator = new AiShipBoardGenerator();
            _enemyCells = aiGenerator.PlaceShips();
            _gameLogic = new GameLogic(_enemyCells, playerCells);

            InitializeBoards();
            SetupGameEvents();
        }

        private void InitializeBoards()
        {
            // Setup player's grid
            for (int i = 0; i < 10; i++)
            {
                PlayerGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                PlayerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                
                for (int j = 0; j < 10; j++)
                {
                    var playerCell = _playerCells[i, j];
                    // Remove from current parent if exists
                    if (playerCell.Parent is Panel parent)
                    {
                        parent.Children.Remove(playerCell);
                    }
                    Grid.SetRow(playerCell, i);
                    Grid.SetColumn(playerCell, j);
                    PlayerGrid.Children.Add(playerCell);
                }
            }

            // Setup enemy's grid
            for (int i = 0; i < 10; i++)
            {
                EnemyGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                EnemyGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                
                for (int j = 0; j < 10; j++)
                {
                    var enemyCell = _enemyCells[i, j];
                    enemyCell.Click += EnemyCell_Click;
                    // Hide enemy ships
                    enemyCell.Background = new ImageBrush(new BitmapImage(
                        new Uri("pack://application:,,,/Images/empty.png")));
                    Grid.SetRow(enemyCell, i);
                    Grid.SetColumn(enemyCell, j);
                    EnemyGrid.Children.Add(enemyCell);
                }
            }
        }

        private void SetupGameEvents()
        {
            _gameLogic.OnHit += (s, e) => 
            {
                MessageBox.Show("Hit!", "Battle Result");
            };

            _gameLogic.OnMiss += (s, e) => 
            {
                MessageBox.Show("Miss!", "Battle Result");
            };

            _gameLogic.OnGameOver += (s, e) => 
            {
                MessageBox.Show("Congratulations! You've won the game!", "Game Over");
                this.Close();
            };
        }

        private void EnemyCell_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var cellInfo = (CellInfo)button.Tag;
            _gameLogic.ProcessShot(cellInfo.Row, cellInfo.Column);
        }
    }
}
