using System.Collections.ObjectModel;
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
       

        public MainWindow()
        {
            InitializeComponent();

            for (int row = 1; row <= 10; row++)
            {
                for (int col = 1; col <= 10; col++)
                {
                    Button cellButton = new Button
                    {
                        Name = $"Button_{row}_{col}",
                        Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/empty.png"))),
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(1),
                        Tag = new CellInfo
                        {
                            Row = row - 1,
                            Column = col - 1,
                            HasShip = false
                        }
                    };

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
            var button = sender as Button;
            var cellInfo = (CellInfo)button.Tag;

            if (cellInfo.HasShip)
            {
                return; // Do nothing if the cell already has a ship
            }

            int x = cellInfo.Column;
            int y = cellInfo.Row;

            if (SmallShipCheckBox.IsChecked == true)
            {
                if (IsFreeArea(x, y, x, y))  // Check only the single tile
                {
                    PlaceShip(button);
                    UpdateLabelAndRadioButton(SmallShipLabel, SmallShipCheckBox);
                }

            }

            if (MediumShipCheckBox.IsChecked == true)
            {
                if (IsFreeArea(x, y, x, y))  // Check only the single tile
                {
                    PlaceShip(button);
                    DisplayAllowedDirection(x, y, MediumSize, ShipDirection.Left);
                    DisplayAllowedDirection(x, y, MediumSize, ShipDirection.Right);
                    DisplayAllowedDirection(x, y, MediumSize, ShipDirection.Up);
                    DisplayAllowedDirection(x, y, MediumSize, ShipDirection.Down);

                   
                    this.KeyDown += OnKeyDown; // Add event handler for arrow key presses
                }
            }


            if (LargeShipCheckBox.IsChecked == true)
            {
                if (IsFreeArea(x, y, x, y))  // Check only the single tile
                {
                    PlaceShip(button);
                    DisplayAllowedDirection(x, y, LargeSize, ShipDirection.Left);
                    DisplayAllowedDirection(x, y, LargeSize, ShipDirection.Right);
                    DisplayAllowedDirection(x, y, LargeSize, ShipDirection.Up);
                    DisplayAllowedDirection(x, y, LargeSize, ShipDirection.Down);


                    this.KeyDown += OnKeyDown; // Add event handler for arrow key presses
                }
                
            }

            if (ExtraLargeShipCheckBox.IsChecked == true)
            {
                //left
                if (IsFreeArea(x - 4, y - 1, x + 1, y + 1))
                {
                    DisplayAllowedDirection(x, y, ExtraLargeSize, ShipDirection.Left);
                }

                //right
                if (IsFreeArea(x - 1, y - 1, x + 4, y + 1))
                {
                    DisplayAllowedDirection(x, y, ExtraLargeSize, ShipDirection.Right);
                }

                //up
                if (IsFreeArea(x - 1, y - 4, x + 1, y + 1))
                {
                    DisplayAllowedDirection(x, y, ExtraLargeSize, ShipDirection.Up);
                }

                //down
                if (IsFreeArea(x - 1, y - 1, x + 1, y + 4))
                {
                    DisplayAllowedDirection(x, y, ExtraLargeSize, ShipDirection.Down);
                }
                //show user option to place ship right
            }

            //A1 -> 3,3

            /*
             * one unit
             *  x =3,3
             *  aaa
             *  axa
             *  aaa
             *  IsFreeArea(2,2, 4,4)
             * 
             * 
             * we want to check ability toplace two-unit ship in any direction
             * 
             * left
             *  y =3.3
             *  x =3,2
             *      addional free space
             *      3,1 2,1 2,2 2,3 2,4 3,4 4,4 4,3 4,2 4,1
             *  aaaa
             *  axya
             *  aaaa
             *  IsFreeArea(2,1, 4,4)
             *  
             * right
             *  y =3.3
             *  x =3,4
             *      addional free space
             *      3,2 2,2 2,3 2,4 2,5 3,5 4,5 4,4 4,3 4,2
             *  aaaa
             *  ayxa
             *  aaaa
             *  IsFreeArea(2,2, 4,5)
             *  
             *  
             * up
             *  y =3.3
             *  x =2,3
             *      addional free space
             *      1,3 1,4 2,4 3,4 4,4 4,3 4,2 3,2 2,2 2,1
             *  aaa
             *  axa
             *  aya
             *  aaa
             *  IsFreeArea(1,3, 4,4)
             *  
             * down
             *  y =3.3
             *  x =4,3
             *  aaa
             *  aya
             *  axa
             *  aaa
             *  IsFreeArea(2,2, 5,4)
             * 
             * 
             * 
            x+1,y+1 
            x+1,y-1 
            x-1,y-1 
            x-1,y+1 
            x-1,y
            x+1,y
            x,y-1
            x,y+1

             how to accees to cell x-1,y
             */

            // Toggle ship placement and change the cell's appearance
            //if (button.Background == Brushes.Transparent)
            //{

            //}
            //else
            //{
            //    button.BorderBrush = Brushes.Black;
            //    button.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/empty.png")));
            //}
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
                int colIndex = x + deltaX*i;
                int rowIndex = y + deltaY*i;

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
        }

        private bool IsFreeArea(int startX, int startY, int endX, int endY)
        {
            int minX = Math.Max(0, startX);
            int minY = Math.Max(0, startY);
            int maxX = Math.Min(9, endX);
            int maxY = Math.Min(9, endY);

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    if (HasCellShip(y, x)) // Assuming (row, col) indexing
                        return false;
                }
            }

            return true;
        }


        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    PlaceShipInDirection(ShipDirection.Left);
                    break;
                case Key.Right:
                    PlaceShipInDirection(ShipDirection.Right);
                    break;
                case Key.Up:
                    PlaceShipInDirection(ShipDirection.Up);
                    break;
                case Key.Down:
                    PlaceShipInDirection(ShipDirection.Down);
                    break;
            }

            this.KeyDown -= OnKeyDown; // Remove event handler after placing the ship
        }



        private void PlaceShipInDirection(ShipDirection direction)
        {
            var lastPlacedButton = _cells.Cast<Button>().FirstOrDefault(btn => (btn.Tag as CellInfo).HasShip);

            if (lastPlacedButton == null)
            {
                return;
            }

            var cellInfo = (CellInfo)lastPlacedButton.Tag;
            int x = cellInfo.Column;
            int y = cellInfo.Row;

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

            for (int i = 1; i < MediumSize; i++)
            {
                int colIndex = x + deltaX * i;
                int rowIndex = y + deltaY * i;

                if (IsValidCellCoordinate(colIndex) && IsValidCellCoordinate(rowIndex))
                {
                    var button = _cells[rowIndex, colIndex];
                    if (!((CellInfo)button.Tag).HasShip) // Ensure the cell is not already occupied by a ship
                    {
                        PlaceShip(button);
                    }
                }
            }

            // Remove arrows after placing the ship
            RemoveArrows(x, y, MediumSize);
        }

        private void RemoveArrows(int x, int y, int shipLength)
        {
            int[] deltaX = { 1, -1, 0, 0 };
            int[] deltaY = { 0, 0, 1, -1 };
            foreach (var dir in Enum.GetValues(typeof(ShipDirection)).Cast<ShipDirection>())
            {
                for (int i = 1; i < shipLength; i++)
                {
                    int colIndex = x + deltaX[(int)dir] * i;
                    int rowIndex = y + deltaY[(int)dir] * i;

                    if (IsValidCellCoordinate(colIndex) && IsValidCellCoordinate(rowIndex))
                    {
                        var button = _cells[rowIndex, colIndex];
                        if (!((CellInfo)button.Tag).HasShip) // Only reset if it doesn't have a ship
                        {
                            button.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/empty.png")));
                            button.BorderBrush = Brushes.Black;
                            button.BorderThickness = new Thickness(1);
                        }
                    }
                }
            }
        }








        //bool IsFreeArea(int startX, int startY, int endX, int endY)
        //{
        //    for (int x = startX; x <= endX; x++)
        //    {
        //        for (int y = startY; y <= endY; y++)
        //        {
        //            int rowIndex = y;
        //            int colIndex = x;

        //            if (!IsValidCellCoordinate(x)
        //                || !IsValidCellCoordinate(y)
        //                || HasCellShip(rowIndex, colIndex))
        //            {
        //                return false;
        //            }
        //        }
        //    }

        //    return true;
        //}

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
}