using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper.Service.DataContracts.Faults
{
    [DataContract]
	public class InvalidSessionFault
	{
        [DataMember]
        public string Message { get; set; }

        public InvalidSessionFault()
        {
            Message = "Session no longer valid";
        }

        public InvalidSessionFault(string message)
        {
            Message = message;
        }
    }
}
