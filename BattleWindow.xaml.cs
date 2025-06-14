// In the InitializeBoards method, modify the enemy grid setup:
for (int i = 0; i < 10; i++)
{
    for (int j = 0; j < 10; j++)
    {
        var enemyCell = _enemyCells[i, j];
        enemyCell.Click += EnemyCell_Click;
        enemyCell.Tag = new CellInfo 
        { 
            Row = i,
            Column = j,
            HasShip = enemyCell.Tag != null  // Preserve ship placement from AiShipBoardGenerator
        };
        enemyCell.Background = new ImageBrush(new BitmapImage(
            new Uri("pack://application:,,,/Images/empty.png")));
        Grid.SetRow(enemyCell, i);
        Grid.SetColumn(enemyCell, j);
        EnemyGrid.Children.Add(enemyCell);
    }
}