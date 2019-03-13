using System.Runtime.Serialization;

namespace MineSweeper.Service.DataContracts
{
    [DataContract]
    public class PlayerData
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public int Points { get; set; }

        [DataMember]
        public bool IsAlive { get; set; }
        
        [DataMember]
        public bool IsReady { get; set; }
    }
}