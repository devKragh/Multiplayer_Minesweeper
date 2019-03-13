
USE dmab0917_1067353;

IF OBJECT_ID('dbo.Field', 'U') IS NOT NULL 
  DROP TABLE dbo.Field;

IF OBJECT_ID('dbo.Game_Player_Relation', 'U') IS NOT NULL 
  DROP TABLE dbo.Game_Player_Relation;

IF OBJECT_ID('dbo.Game', 'U') IS NOT NULL 
  DROP TABLE dbo.Game;

IF OBJECT_ID('dbo.Account', 'U') IS NOT NULL 
  DROP TABLE dbo.Account;

CREATE TABLE Account(
	id INT PRIMARY KEY IDENTITY(1,1),
	salt VARCHAR(50) NOT NULL,
	username VARCHAR(14) UNIQUE NOT NULL,
	[password] VARCHAR(256) NOT NULL,
	sessionkey VARCHAR(36) DEFAULT NULL,
	rankpoints INT NOT NULL DEFAULT 0,
	active BIT NOT NULL DEFAULT 1
)

CREATE TABLE Game(
	id INT PRIMARY KEY IDENTITY(1,1),
	is_completed BIT NOT NULL DEFAULT 0,
	is_started BIT NOT NULL DEFAULT 0,
	height INT NOT NULL DEFAULT 16,
	width INT NOT NULL DEFAULT 30,
	winning_player_id INT DEFAULT NULL,
	[type] VARCHAR(30) default NULL,
	FOREIGN KEY (winning_player_id) REFERENCES Account(id)
		ON UPDATE NO ACTION
		ON DELETE NO ACTION
)

CREATE TABLE Game_Player_Relation(
	player_id int NOT NULL,
	game_id int NOT NULL,
	ready BIT DEFAULT 0,
	is_alive BIT DEFAULT 1,
	PRIMARY KEY (player_id, game_id),
	FOREIGN KEY (player_id) REFERENCES Account(id)
		ON UPDATE NO ACTION
		ON DELETE NO ACTION,
	FOREIGN KEY (game_id) REFERENCES Game(id)
		ON UPDATE CASCADE
		ON DELETE CASCADE
)

CREATE TABLE Field(
	x_coordinate INT NOT NULL,
	y_coordinate INT NOT NULL,
	pressed_by_user_id INT DEFAULT NULL,
	is_mine BIT NOT NULL DEFAULT 0,
	game_id INT NOT NULL,
	time_pressed DATETIME DEFAULT NULL,
	PRIMARY KEY (x_coordinate, y_coordinate, game_id),
	FOREIGN KEY (game_id) REFERENCES Game(id)
		ON DELETE CASCADE
		ON UPDATE CASCADE,
	FOREIGN KEY (pressed_by_user_id) REFERENCES Account(id)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
)

GO

IF EXISTS(select * FROM sys.views where name = 'Game_queue')
	DROP VIEW dbo.Game_queue;

GO

IF EXISTS(select * FROM sys.views where name = 'Completed_Game_Winners')
	DROP VIEW dbo.Completed_Game_Winners;

GO

IF EXISTS(select * FROM sys.views where name = 'Completed_Games')
	DROP VIEW dbo.completed_games;

GO

IF EXISTS(select * FROM sys.views where name = 'Player_Stats')
	DROP VIEW dbo.Player_Stats;

GO

IF EXISTS(select * FROM sys.views where name = 'QuickMatch_Queue')
	DROP VIEW dbo.QuickMatch_Queue;

GO

IF EXISTS(select * FROM sys.views where name = 'FreeForAll_Queue')
	DROP VIEW dbo.FreeForAll_Queue;

GO

CREATE VIEW Completed_Game_Winners as
	SELECT dbo.Game.id as game_id, dbo.Game.winning_player_id as winning_player_id
	FROM dbo.Game
	WHERE (dbo.Game.is_completed = 1)
GO

CREATE VIEW Completed_Games as
	SELECT dbo.Game_Player_Relation.game_id as game_id, dbo.Game_Player_Relation.player_id as player_id
	FROM dbo.Game INNER JOIN
		 dbo.Game_Player_Relation ON dbo.Game.id = dbo.Game_Player_Relation.game_id
	WHERE (dbo.Game.is_completed = 1)
GO

CREATE VIEW Player_Stats as
	SELECT Account.id AS player_id, 
	(SELECT COUNT(winning_player_id) FROM dbo.Completed_Game_Winners WHERE winning_player_id = Account.id) as wins,
	(SELECT COUNT(player_id) FROM dbo.Completed_Games WHERE player_id = Account.id) as total
	FROM Account GROUP BY Account.id
GO

CREATE VIEW FreeForAll_Queue as
	SELECT Game.Id 
	FROM Game 
	WHERE [type] = 'FreeForAll' AND is_completed = 0 AND is_started = 0
GO

--CREATE VIEW Game_queue AS
--SELECT id, player_1_id,player_2_id, is_started, is_completed
--FROM Game WITH (READPAST)
--WHERE (is_completed = 0) AND (is_started = 0) AND (player_1_id IS NULL OR player_2_id IS NULL)

--GO

CREATE VIEW QuickMatch_Queue as
	SELECT * 
	FROM Game 
	WHERE [type] = 'QuickMatch' AND is_completed = 0 AND is_started = 0 AND (
	(SELECT COUNT(Game_Player_Relation.game_id) FROM Game_Player_Relation WHERE game_id = Game.id GROUP BY Game_Player_Relation.game_id) = 1 OR
	(SELECT COUNT(Game_Player_Relation.game_id) FROM Game_Player_Relation WHERE game_id = Game.id GROUP BY Game_Player_Relation.game_id) IS NULL -- Betyder = 0 med Count
)
GO

