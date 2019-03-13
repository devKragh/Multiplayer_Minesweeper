using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper.Service.DataContracts.Faults
{
    [DataContract]
    public class DBFault
    {
        [DataMember]
        public string Message { get; set; }

        public DBFault()
        {
            Message = "Error with Database";
        }

        public DBFault(string message)
        {
            Message = message;
        }
    }
}
