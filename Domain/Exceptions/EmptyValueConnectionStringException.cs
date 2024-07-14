using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
	public class EmptyValueConnectionStringException : Exception
	{
		public EmptyValueConnectionStringException() { }
		public EmptyValueConnectionStringException(string message) : base(message) { }
		public EmptyValueConnectionStringException(string message, Exception inner) : base(message, inner) { }
	}
}
