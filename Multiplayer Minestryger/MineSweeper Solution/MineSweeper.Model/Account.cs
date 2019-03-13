using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper.Model
{
    [DataContract]
    public class Account
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Username { get; set; }

        public string Password { get; set; }

        public string Salt { get; set; }

        [DataMember]
        public int Rankpoints { get; set; }

        public Guid SessionKey { get; set; }

        public int Points { get; set; }

        [DataMember]
        public bool IsAlive { get; set; }

		[DataMember]
		public bool Active { get; set; }

		public Account()
		{
			Id = -1;
            IsAlive = true;
		}
    }
}
