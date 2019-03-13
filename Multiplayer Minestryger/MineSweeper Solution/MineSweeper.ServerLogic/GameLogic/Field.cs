using System;
using System.Collections.Generic;

namespace MineSweeper.ServerLogic.GameLogic
{
    public class Field
    {
        public bool IsMine { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int PressedByPlayerId { get; set; }
        public DateTime? TimePressed { get; set; }
        public bool IsPressed { get; set; }
        public List<Field> AdjacentFields { get; set; }

        public Field(int x, int y)
        {
            X = x;
            Y = y;
            IsMine = false;
            AdjacentFields = new List<Field>();
            IsPressed = false;
            PressedByPlayerId = -1;
        }

        public int AdjacentMines()
        {
            int mines = 0;
            foreach (Field adjacentField in AdjacentFields)
            {
                if (adjacentField.IsMine)
                {
                    mines++;
                }
            }
            return mines;
        }


    }
}
