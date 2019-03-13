using System.Runtime.Serialization;

namespace MineSweeper.Service.DataContracts.Faults
{
    [DataContract]
    public class InvalidUserCredentialsFault
    {
        [DataMember]
        public string Message { get; set; }

        public InvalidUserCredentialsFault(string message)
        {
            Message = message;
        }
    }
}