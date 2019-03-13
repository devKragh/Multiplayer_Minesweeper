using MineSweeper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper.ServerLogic.GameLogic
{
    public class Game
    {
        public int Id { get; set; }
        public Account Player { get; set; }
        public List<Account> OpponentPlayers { get; set; }
        public bool IsCompleted { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public Field[,] Minefield { get; set; }
        public bool IsStarted { get; set; }
        public int MineAmount { get; set; }
        public Account WinnerPlayer { get; set; }
        public GameType GameType { get; set; }

        public Game()
        {
            Id = -1;
            IsCompleted = false;
            Height = 16;
            Width = 30;
            MineAmount = 50;
            Minefield = new Field[Width,Height];
            IsStarted = false;
            WinnerPlayer = null;
            OpponentPlayers = new List<Account>();
            GameType = GameType.Unknown;
            InstantiateFields();
            ConnectFields();
        }

        private void InstantiateFields()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Minefield[x, y] = new Field(x, y);
                }
            }
        }


        private void ConnectFields()
        {
            foreach (Field field in Minefield)
            {
                AddAdjacentFields(field);
            }
        }

        private void AddAdjacentFields(Field field)
        {
            TryAddField(field, field.X + 1, field.Y + 1);
            TryAddField(field, field.X, field.Y + 1);
            TryAddField(field, field.X - 1, field.Y + 1);
            TryAddField(field, field.X + 1, field.Y);
            TryAddField(field, field.X - 1, field.Y);
            TryAddField(field, field.X + 1, field.Y - 1);
            TryAddField(field, field.X, field.Y - 1);
            TryAddField(field, field.X - 1, field.Y - 1);
        }

        private void TryAddField(Field coreField, int x, int y)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
            {
                coreField.AdjacentFields.Add(Minefield[x, y]);
            }
        }
    }
}
