using MineSweeper.Model;
using MineSweeper.ServerLogic.BusinessLogic;
using MineSweeper.ServerLogic.Exceptions;
using MineSweeper.ServerLogic.GameLogic;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;
using System.Transactions;

namespace MineSweeper.ServerLogic.DB
{
    public class GameDB : IGameDB
    {
        private string connectionString;

        public GameDB()
        {
            connectionString = ConfigurationManager.ConnectionStrings["localDB"].ConnectionString;
            //connectionString = "Data Source=kraka.ucn.dk;Initial Catalog=dmab0917_1067353;User id=dmab0917_1067353;Password=Password1!"; // TODO Få configurationmanager til at virke
        }

        public void JoinGame(Game game)
        {
            try
            {
                TransactionOptions transactionOptions = new TransactionOptions();
                transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
                using (TransactionScope transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, transactionOptions))
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        using (SqlCommand getActiveGamesCmd = new SqlCommand(String.Format("select top 1 * from {0}_Queue (READPAST)", game.GameType.ToString()), connection))
                        {
                            SqlDataReader result = getActiveGamesCmd.ExecuteReader();
                            // If a game is available then join that otherwise insert new game and use that
                            if (result.Read())
                            {
                                game.Id = result.GetInt32(0);
                                result.Close();
                            }
                            else
                            {
                                result.Close();
                                InsertNewGame(game, connection);
                            }

                            using (SqlCommand addPlayerCmd = new SqlCommand(String.Format("INSERT INTO dbo.Game_Player_Relation VALUES( {0}, {1}, DEFAULT, DEFAULT)", game.Player.Id, game.Id), connection))
                            {
                                int rowsAffected = -1;
                                rowsAffected = addPlayerCmd.ExecuteNonQuery();
                                if (rowsAffected != 1)
                                {
                                    throw new DBException("Game_Player_Relation insert failed");
                                }
                            }
                        }
                    }
                    transactionScope.Complete();
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void InsertNewGame(Game game, SqlConnection connection)
        {
            using (SqlCommand insertNewGameCmd = new SqlCommand("insert into Game values (DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT, @Type); SELECT SCOPE_IDENTITY() AS int", connection))  //Hardcoded size
            {
                insertNewGameCmd.Parameters.AddWithValue("Type", game.GameType.ToString());
                int id = Convert.ToInt32((insertNewGameCmd.ExecuteScalar()));
                game.Id = id;
                InsertFields(game, connection);
            }
        }

        private void InsertFields(Game game, SqlConnection connection)
        {
            StringBuilder queries = new StringBuilder();
            queries.Append("insert into Field values ");
            foreach (Field field in game.Minefield)
            {
                queries.Append(string.Format("({0},{1},null,0,{2},null),", field.X, field.Y, game.Id));
            }
            queries.Remove(queries.Length - 1, 1);
            using (SqlCommand insertFieldCmd = new SqlCommand(queries.ToString(), connection))
            {
                int rowsAffected = -1;
                rowsAffected = insertFieldCmd.ExecuteNonQuery();
                if (rowsAffected != (game.Height * game.Width))
                {
                    throw new DBException("Field insert failed");
                }
            }
        }

        public void StartGame(Game game)
        {
            TransactionOptions transactionOptions = new TransactionOptions();
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.RepeatableRead;
            using (TransactionScope transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, transactionOptions))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand checkIfGameIsStartedCmd = new SqlCommand("SELECT is_started FROM Game WHERE id = " + game.Id, connection))
                    {
                        SqlDataReader result = checkIfGameIsStartedCmd.ExecuteReader();
                        if (result.Read())
                        {
                            if (result.GetBoolean(0) == false)
                            {
                                result.Close();
                                using (SqlCommand setGameStartedCmd = new SqlCommand("update Game set is_started=@IsStarted where id=@GameID", connection))
                                {

                                    int gameRowsAffected = -1;
                                    setGameStartedCmd.Parameters.AddWithValue("GameID", game.Id);
                                    setGameStartedCmd.Parameters.AddWithValue("IsStarted", true);
                                    gameRowsAffected = setGameStartedCmd.ExecuteNonQuery();
                                    if (gameRowsAffected != 1)
                                    {
                                        throw new DBException("Game update failed");
                                    }

                                    PlaceMines(game, connection);
                                }
                            }
                            result.Close();
                        }
                    }
                    transactionScope.Complete();
                }

            }
        }


        private void PlaceMines(Game game, SqlConnection connection)
        {
            StringBuilder queries = new StringBuilder();
            bool hasMines = false;
            foreach (Field field in game.Minefield)
            {
                if (field.IsMine)
                {
                    queries.Append(String.Format("update Field set is_mine=1 where x_coordinate={0} and y_coordinate={1} and game_id={2};", field.X, field.Y, game.Id));
                    hasMines = true;
                }
            }
            if (hasMines)
            {
                using (SqlCommand placeMinesCmd = new SqlCommand(queries.ToString(), connection))
                {
                    int rowsAffected = placeMinesCmd.ExecuteNonQuery();
                    if (rowsAffected != game.MineAmount)
                    {
                        throw new DBException("Field update failed");
                    }
                }
            }
        }

        public void UpdateGameObject(Game game)
        {
            TransactionOptions transactionOptions = new TransactionOptions();
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            //Is allowed to be read committed, even though it can cause dirty reads temporarily
            //Becuase everytime something happens in the game, this method (and transactionscope)
            //will run again, updating the game object completly again. And having a low transaction
            //level avoids making read/write locks and makes the game run smoother
            using (TransactionScope transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, transactionOptions))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand getGameDataCmd = new SqlCommand("SELECT * FROM Game WHERE id = " + game.Id, connection))
                    {
                        getGameDataCmd.Parameters.AddWithValue("GameID", game.Id);
                        SqlDataReader result = getGameDataCmd.ExecuteReader();
                        if (result.Read())
                        {
                            game.IsCompleted = result.GetBoolean(1);
                            game.IsStarted = result.GetBoolean(2);
                        }
                        result.Close();
                    }

                    using (SqlCommand getPlayersCmd = new SqlCommand("SELECT * FROM Game_Player_Relation WHERE game_id = " + game.Id, connection))
                    {

                        SqlDataReader result = getPlayersCmd.ExecuteReader();
                        while (result.Read())
                        {
                            int accountId = result.GetInt32(0);
                            if (accountId == game.Player.Id)
                            {
                                game.Player.IsAlive = result.GetBoolean(3);
                            }
                            else
                            {
                                Account opponentPlayer = game.OpponentPlayers.Find(x => x.Id == accountId);
                                if (opponentPlayer == null)
                                {
                                    IAccountController accountController = new AccountController();
                                    opponentPlayer = accountController.GetAccount(accountId);
                                    game.OpponentPlayers.Add(opponentPlayer);
                                }
                                opponentPlayer.IsAlive = result.GetBoolean(3);
                            }
                        }
                        result.Close();
                    }
                    using (SqlCommand getFieldDataCmd = new SqlCommand("select * from Field where game_id = @GameID", connection))
                    {
                        getFieldDataCmd.Parameters.AddWithValue("GameID", game.Id);
                        SqlDataReader fieldsResult = getFieldDataCmd.ExecuteReader();
                        while (fieldsResult.Read())
                        {
                            int x = fieldsResult.GetInt32(0);
                            int y = fieldsResult.GetInt32(1);
                            Field field = game.Minefield[x, y];
                            if (!fieldsResult.IsDBNull(2))
                            {
                                int pressedByUserId = fieldsResult.GetInt32(2);
                                field.PressedByPlayerId = pressedByUserId;
                            }
                            else
                            {
                                field.PressedByPlayerId = -1;
                            }
                            bool isMine = fieldsResult.GetBoolean(3);
                            if (!fieldsResult.IsDBNull(5))
                            {
                                field.TimePressed = fieldsResult.GetDateTime(5);
                                field.IsPressed = true;
                            }
                            field.IsMine = isMine;
                        }
                        fieldsResult.Close();
                    }
                    transactionScope.Complete();
                }
            }
        }

        public void DeleteGame(int id) //Was added for test purposes and is not used in the application
        {
            TransactionOptions transactionOptions = new TransactionOptions();
            transactionOptions.IsolationLevel = IsolationLevel.ReadCommitted;
            //Was added for test purposes and is not used in the application
            //So isolationslevel is not considered
            using (TransactionScope transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, transactionOptions))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand deleteGameCmd = new SqlCommand("delete from Game where id=@GameID", connection))
                    {
                        deleteGameCmd.Parameters.AddWithValue("GameID", id);
                        int rowsAffected = deleteGameCmd.ExecuteNonQuery();
                        if (rowsAffected != 1)
                        {
                            throw new DBException("Could not delete game.");
                        }
                        transactionScope.Complete();
                    }
                }
            }
        }

        public void MakeMove(Game game, List<Field> fieldsToActivate)
        {
            try
            {
                TransactionOptions transactionOptions = new TransactionOptions();
                transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
                using (TransactionScope transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, transactionOptions))
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        StringBuilder queries = new StringBuilder();
                        Field userPressedField = fieldsToActivate[0];
                        queries.Append(string.Format("UPDATE Field SET time_pressed=GETDATE(),pressed_by_user_id={0} WHERE x_coordinate={1} AND y_coordinate={2} AND game_id={3} AND time_pressed IS NULL AND pressed_by_user_id IS NULL ", userPressedField.PressedByPlayerId, userPressedField.X, userPressedField.Y, game.Id));
                        for (int i = 1; i < fieldsToActivate.Count; i++)
                        {
                            Field field = fieldsToActivate[i];
                            queries.Append(string.Format("UPDATE Field SET time_pressed=GETDATE(),pressed_by_user_id=null where x_coordinate={0} and y_coordinate={1} and game_id={2} AND time_pressed IS NULL AND pressed_by_user_id IS NULL;", field.X, field.Y, game.Id));
                        }
                        using (SqlCommand updateFieldsCmd = new SqlCommand(queries.ToString(), connection))
                        {
                            int rowsAffected = updateFieldsCmd.ExecuteNonQuery();
                        }
                    }
                    transactionScope.Complete();
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
        }

        public void EndGame(Game game)
        {
            TransactionOptions transactionOptions = new TransactionOptions();
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            using (TransactionScope transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, transactionOptions))
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand setGameCompleted = new SqlCommand("update Game set is_completed=1, winning_player_id=@WinningPlayerID where id=@GameID", connection))
                    {

                        if (game.WinnerPlayer == null)
                        {
                            setGameCompleted.Parameters.AddWithValue("WinningPlayerID", DBNull.Value);
                        }
                        else
                        {
                            setGameCompleted.Parameters.AddWithValue("WinningPlayerID", game.WinnerPlayer.Id);
                        }
                        setGameCompleted.Parameters.AddWithValue("GameID", game.Id);
                        int rowsAffected = setGameCompleted.ExecuteNonQuery();
                        if (rowsAffected != 1)
                        {
                            // throw new DBException("Game update failed");

                        }
                    }
                }
                transactionScope.Complete();
            }
        }

        public int GetWinsByUserId(int accountId)
        {
            int wins = 0;

            TransactionOptions transactionOptions = new TransactionOptions();
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            using (TransactionScope transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, transactionOptions))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand getWinsCmd = new SqlCommand(("select wins from Player_stats where player_id = " + accountId), connection))
                    {
                        SqlDataReader result = getWinsCmd.ExecuteReader();
                        if (result.Read())
                        {

                            wins = result.GetInt32(0);
                        }
                    }
                }
            }

            return wins;
        }

        public int GetLossesByUserId(int accountId)
        {
            int losses = 0;

            TransactionOptions transactionOptions = new TransactionOptions();
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            using (TransactionScope transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, transactionOptions))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand getWinsCmd = new SqlCommand(("select total, wins from Player_stats where player_id = " + accountId), connection))
                    {
                        SqlDataReader result = getWinsCmd.ExecuteReader();
                        if (result.Read())
                        {
                            losses = result.GetInt32(0) - result.GetInt32(1);
                        }
                    }
                }
            }

            return losses;
        }

        //Not Implemented
        public void JoinActiveGame(Game game)
        {
            throw new NotImplementedException();
            TransactionOptions transactionOptions = new TransactionOptions();
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            using (TransactionScope transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, transactionOptions))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand getWinsCmd = new SqlCommand(("select total, wins from Player_stats where player_id = " + "accountId" /* Use account Id */), connection))
                    {
                    }
                }

                transactionScope.Complete();
            }
        }

        public void SetPlayerReady(Game game)
        {
            TransactionOptions transactionOptions = new TransactionOptions();
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted; // Execution of a single Query, not dependant on other tables/rows 
            using (TransactionScope transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, transactionOptions))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand updatePlayerInfo = new SqlCommand(string.Format("UPDATE Game_Player_Relation SET ready = 1 WHERE player_id = {0} AND game_id = {1}", game.Player.Id, game.Id), connection))
                    {
                        int rowsAffected = updatePlayerInfo.ExecuteNonQuery();
                        if (rowsAffected != 1)
                        {
                            throw new DBException("Game_Player_Relation update failed");
                        }
                    }
                }
                transactionScope.Complete();
            }
        }

        public bool CheckAllPlayerReadiness()
        {
            bool allPlayersReady = true;
            TransactionOptions transactionOptions = new TransactionOptions();
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.RepeatableRead;
            //Needs to make sure, all players are added to the game when checking if they are ready
            using (TransactionScope transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, transactionOptions))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand setPlayerReadyCmd = new SqlCommand("SELECT * FROM Game_Player_Relation", connection))
                    {
                        using (SqlDataReader result = setPlayerReadyCmd.ExecuteReader())
                        {
                            while (result.Read())
                            {
                                if (!result.GetBoolean(2))
                                {
                                    allPlayersReady = false;
                                }
                            }
                        }
                    }
                }
                transactionScope.Complete();
            }
            return allPlayersReady;
        }

        public void SetPlayerDead(Game game)
        {
            TransactionOptions transactionOptions = new TransactionOptions();
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            using (TransactionScope transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, transactionOptions))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand setPlayerDeadCmd = new SqlCommand(string.Format("UPDATE Game_Player_Relation SET is_alive = 0 WHERE player_id = {0} AND game_id = {1} AND is_alive = 1", game.Player.Id, game.Id), connection))
                    {
                        int rowsAffected = setPlayerDeadCmd.ExecuteNonQuery();
                    }
                }
                transactionScope.Complete();
            }
        }

        public void UnjoinGame(Game game)
        {
            TransactionOptions transactionOptions = new TransactionOptions();
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            using (TransactionScope transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, transactionOptions))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand removePlayerFromGame = new SqlCommand(string.Format("DELETE FROM Game_Player_Relation WHERE player_id={0} AND game_id={1}", game.Player.Id, game.Id), connection))
                    {
                        removePlayerFromGame.ExecuteNonQuery();
                    }
                }
                transactionScope.Complete();
            }
        }
    }
}

