﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
	public class AccountNotFoundException : Exception
	{
		public AccountNotFoundException() { }
		public AccountNotFoundException(string message) : base(message) { }
		public AccountNotFoundException(string message, Exception inner) : base(message, inner) { }
	}
}
