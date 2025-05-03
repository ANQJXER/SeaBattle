using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SeBattle2.Logic
{
    internal class AiShipBoardGenerator
    {
        public Button[,] PlaceShips()
        {
            var board = new Button[10, 10];
            ShipPlacementChecker shipPlacementChecker = new ShipPlacementChecker(board);

            /*
            Algo for placing ships for PC
               area - 10 x 10
               ships
                   4 x 1
                   3 x 2
                   2 x 3
                   1 x 4


               Randomly select nummber 1..10 -> X	
               Randomly select nummber 1..10 -> Y
               (X,Y) 
                   Execute all checking logic from design screen
                 _shipPlacementChecker.Check(X, Y, shipSize);.

               For one-unit -> done!

               To select direction: 1..4
               up - 1 
               donw -2 right - 3
               left - 4

               Execute all checking logic from design screen

            */

            return board;
        }
    }
}
