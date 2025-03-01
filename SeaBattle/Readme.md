Develop components for game
    Ship
        - ShipType 
            - 1
            - 2
            - 3
            - 4
        - ShipDirection
            - Horizontal
            - Vertical
        - ShipPosition
            - X
            - Y
        - ShipStatus
            - Alive
            - Dead
    Board
        - BoardSize
            - X
            - Y
        - BoardPosition
            - X
            - Y
        - BoardStatus
            - Empty
            - Ship
            - Hit
            - Miss
    Game
        - GameStatus
            - Start
            - End
        - GameMode
            - SinglePlayer
        - GameResult
            - Win
            - Lose
            - Draw
        - GameTurn
            - Player
            - AI
        - GameScore
            - Player
            - AI
        - GameTime
            - Start
            - End
        - GameHistory
            - Player
            - AI
        - GameLog
            - Player
            - AI
        - GameSetting
            - BoardSize
            - ShipType
            - ShipDirection
            - GameMode
            - GameTime
            - GameHistory
            - GameLog
        - GameRule
            - ShipPlacement
            - ShipSunk
            - GameEnd
        - GameEngine
            - GameStart
            - GameEnd
            - GameTurn
            - GameScore
            - GameTime
            - GameHistory
            - GameLog
            - GameSetting
            - GameRule
        - GameUI
            - GameStart
            - GameEnd
            - GameTurn
            - GameScore
            - GameTime
            - GameHistory
            - GameLog
            - GameSetting
            - GameRule
        - GameAI
        - GameDatabase
        - GameTest
            - GameStart
            - GameEnd
            - GameTurn
            - GameScore
            - GameTime
            - GameHistory
            - GameLog
            - GameSetting
            - GameRule




Game
    Two boards
        Each board has 10x10 cells
            Each cell has 4 states: Empty, Ship, Hit, Miss
        Each board contains 10 ships
            1 ship of size 4
            2 ships of size 3
            3 ships of size 2
            4 ships of size 1
            Each ship has 2 states: 
                - Alive
                    - all cells have status Ship
                - Dead
                    - all cells have status Hit
                - Partialy hit
                    - some cells have status Hit
                

Two screens
    Design screen
        - Place ships
    Play screen
        - two boards
            - one for player
            - one for AI
