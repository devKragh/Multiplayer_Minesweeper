using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper.Service.DataContracts
{
    [DataContract]
    public class GameData
    {
		[DataMember]
		public int GameId { get; set; }
		[DataMember]
        public PlayerData Player { get; set; }
        [DataMember]
        public List<PlayerData> OpponentPlayers { get; set; }
        [DataMember]
        public int Height { get; set; }
        [DataMember]
        public int Width { get; set; }
        [DataMember]
        public FieldData[][] JaggedMinefield { get; set; }
        [DataMember]
        public int MineAmount { get; set; }
        public FieldData[,] Minefield { get; set; }
    }
}
