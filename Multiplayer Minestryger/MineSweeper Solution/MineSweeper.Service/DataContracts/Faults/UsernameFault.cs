﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper.Service.DataContracts.Faults
{
	[DataContract]
	public class UsernameFault
	{
		[DataMember]
		public string Message { get; set; }

		public UsernameFault(string message)
		{
			Message = message;
		}
	}
}

