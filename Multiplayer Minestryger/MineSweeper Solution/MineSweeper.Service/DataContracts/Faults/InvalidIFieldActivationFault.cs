using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper.Service.DataContracts.Faults
{
    [DataContract]
    public class InvalidIFieldActivationFault
    {
        [DataMember]
        public string Message { get; set; }

        public InvalidIFieldActivationFault(string message)
        {
            Message = message;
        }
    }
}
