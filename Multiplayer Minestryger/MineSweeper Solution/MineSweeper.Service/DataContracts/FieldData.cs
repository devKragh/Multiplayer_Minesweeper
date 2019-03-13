using System;
using System.Runtime.Serialization;

namespace MineSweeper.Service.DataContracts
{
    [DataContract]
    public class FieldData
    {
        [DataMember]
        public int X { get; set; }
        [DataMember]
        public int Y { get; set; }
        [DataMember]
        public PlayerData PressedByPlayer { get; set; }
        [DataMember]
        public bool IsPressed { get; set; }
        public bool IsFlag { get; set; }
        [DataMember]
        public int AdjacentMines { get; set; }
        [DataMember]
        public bool IsMine { get; set; } //this property will not be set by the service until the game is completed
    }
}