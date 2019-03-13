using MineSweeper.Model;
using MineSweeper.ServerLogic.DB;
using MineSweeper.ServerLogic.Exceptions;
using System;
using System.Collections.Generic;

namespace MineSweeper.ServerLogic.GameLogic
{
    public class GameController : IGameController
    {
        public Game Game { get; set; }
        private IGameDB gameDB;
        public bool AllPlayersReady { get; set; }

        public GameController(Account account)
        {
            Game = new Game();
            Game.Player = account;
            gameDB = new GameDB();
        }

        public void TryActivateField(int x, int y)
        {
            if (x < 0 && x >= Game.Width && y < 0 && y >= Game.Height)
            {
                throw new InvalidFieldActivationException("X: " + x + ", Y: " + y + " is not a valid field");
            }
            if (!Game.IsCompleted && AllPlayersReady && Game.Player.IsAlive)
            {
                Field pressedField = Game.Minefield[x, y];
                if (!Game.IsStarted)
                {
                    SetRandomMines(pressedField);
                    gameDB.StartGame(Game);
                    Update();
                }

                pressedField.PressedByPlayerId = Game.Player.Id;
                pressedField.IsPressed = true;

                List<Field> fieldsToActivate = new List<Field>()
                {
                    pressedField
                };

                if (!pressedField.IsMine)
                {
                    RecursiveFieldActivation(pressedField, fieldsToActivate);
                }
                else if (pressedField.IsMine)
                {
                    Game.Player.IsAlive = false;
                    gameDB.SetPlayerDead(Game);
                }
                gameDB.MakeMove(Game, fieldsToActivate);
            }
        }

        private bool CheckGameEndingMove()
        {
            bool noClearFieldNeglected = true;

            switch (Game.GameType)
            {

                case GameType.QuickMatch:

                    foreach (Field field in Game.Minefield)
                    {
                        if (field.IsMine && field.IsPressed)
                        {
                            Game.IsCompleted = true;
                            return true;
                        }
                        else if (!field.IsPressed && !field.IsMine)
                        {
                            noClearFieldNeglected = false;
                        }
                    }
                    Game.IsCompleted = noClearFieldNeglected;
                    return noClearFieldNeglected;


                case GameType.FreeForAll:

                    if (Game.IsStarted)
                    {
                        List<Account> players = new List<Account>(Game.OpponentPlayers);
                        players.Add(Game.Player);
                        int alivePlayers = 0;
                        foreach (Account player in players)
                        {

                            if (player.IsAlive)
                            {
                                alivePlayers++;
                            }

                        }
                        if (alivePlayers == 1)
                        {
                            return true;
                        }

                        foreach (Field field in Game.Minefield)
                        {
                            if (!field.IsPressed && !field.IsMine)
                            {
                                noClearFieldNeglected = false;
                            }
                        }
                        Game.IsCompleted = noClearFieldNeglected;
                        return noClearFieldNeglected;
                    }

                    return false;

                default:
                    return true;


            }


        }

        private void CalculateWinner()
        {

            switch (Game.GameType)
            {
                case GameType.QuickMatch:

                    bool mineActivated = false;
                    foreach (Field field in Game.Minefield)
                    {
                        if (field.IsMine && field.IsPressed)
                        {
                            if (Game.OpponentPlayers.Count > 0)
                            {
                                if (field.PressedByPlayerId == Game.OpponentPlayers[0].Id)
                                {
                                    Game.WinnerPlayer = Game.Player;
                                }
                            }
                            if (field.PressedByPlayerId == Game.Player.Id && Game.OpponentPlayers.Count > 0)
                            {
                                Game.WinnerPlayer = Game.OpponentPlayers[0];
                            }
                            mineActivated = true;
                        }
                    }
                    if (!mineActivated)
                    {

                        Game.WinnerPlayer = CalculatePoints();
                    }
                    break;

                case GameType.FreeForAll:
                    List<Account> players = new List<Account>(Game.OpponentPlayers);
                    players.Add(Game.Player);
                    List<Account> alivePlayers = new List<Account>();

                    foreach (Account player in players)
                    {
                        if (player.IsAlive)
                        {
                            alivePlayers.Add(player);
                        }
                    }
                    Account aliveTopScorer = new Account()
                    {
                        Id = -1,
                        Points = -1
                    };
                    foreach (Account player in alivePlayers)
                    {
                        if (player.Points > aliveTopScorer.Points)
                        {
                            aliveTopScorer = player;
                        }

                    }
                    Game.WinnerPlayer = aliveTopScorer;

                    break;
            }
        }

        private void RecursiveFieldActivation(Field pressedField, List<Field> fieldsToActivate)
        {
            if (pressedField.AdjacentMines() == 0)
            {
                foreach (Field adjacentField in pressedField.AdjacentFields)
                {
                    if (!adjacentField.IsPressed)
                    {
                        adjacentField.IsPressed = true;
                        fieldsToActivate.Add(adjacentField);
                        RecursiveFieldActivation(adjacentField, fieldsToActivate);
                    }
                }
            }
        }

        private void SetRandomMines(Field pressedField)
        {
            List<Field> blackList = new List<Field>()
            {
                Game.Minefield[pressedField.X, pressedField.Y],
                Game.Minefield[pressedField.X, pressedField.Y - 1],
                Game.Minefield[pressedField.X, pressedField.Y + 1],
                Game.Minefield[pressedField.X -1, pressedField.Y],
                Game.Minefield[pressedField.X-1, pressedField.Y - 1],
                Game.Minefield[pressedField.X-1, pressedField.Y + 1],
                Game.Minefield[pressedField.X+1, pressedField.Y],
                Game.Minefield[pressedField.X+1, pressedField.Y - 1],
                Game.Minefield[pressedField.X+1, pressedField.Y + 1]
            };
            int minesPlaced = 0;
            Random random = new Random(DateTime.Now.Second);
            while (minesPlaced < Game.MineAmount)
            {
                int randX = random.Next(Game.Width);
                int randY = random.Next(Game.Height);
                Field field = Game.Minefield[randX, randY];
                if (!field.IsMine)
                {
                    bool canPlace = true;
                    foreach (Field blackListField in blackList)
                    {
                        if (blackListField.X == field.X && blackListField.Y == field.Y)
                        {
                            canPlace = false;
                        }
                    }
                    if (canPlace)
                    {
                        Game.Minefield[randX, randY].IsMine = true;
                        minesPlaced++;
                    }
                }
            }
        }

        public void Update()
        {
            gameDB.UpdateGameObject(Game);
            if (CheckGameEndingMove())
            {
                CalculateWinner();
                EndGame();
            }
        }

        private Account CalculatePoints()
        {

            Account playerWithMostPoints = new Account();
            playerWithMostPoints.Points = -1;
            playerWithMostPoints.Id = -1;
            List<Account> players = Game.OpponentPlayers;
            players.Add(Game.Player);
            foreach (Field field in Game.Minefield)
            {
                if (field.IsPressed)
                {
                    Account player = players.Find(x => x.Id == field.PressedByPlayerId);
                    if (player != null)
                    {
                        player.Points++;
                        if (player.Points > playerWithMostPoints.Points)
                        {
                            playerWithMostPoints = player;
                        }
                    }
                }
            }

            return playerWithMostPoints;
        }

        public void JoinActiveGame()
        {
            // gameDB.JoinActiveGame(Game.Player.Id, Game);
        }

        public void JoinNewGame(GameType gameType)
        {
            Game.GameType = gameType;
            gameDB.JoinGame(Game);
        }

        public void Ready()
        {
            gameDB.SetPlayerReady(Game);
        }

        public bool CheckAllPlayerReadiness()
        {
            AllPlayersReady = gameDB.CheckAllPlayerReadiness();
            return AllPlayersReady;
        }

        public void EndGame()
        {
            Game.IsCompleted = true;
            gameDB.EndGame(Game);
        }

        public void UnjoinGame()
        {
            Update();
            if (!Game.IsStarted)
            {
                gameDB.UnjoinGame(Game);
            }
        }
    }
}

