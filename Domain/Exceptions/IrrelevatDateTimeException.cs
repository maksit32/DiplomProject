using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
	public class IrrelevatDateTimeException : Exception
	{
		public IrrelevatDateTimeException() { }
		public IrrelevatDateTimeException(string message) : base(message) { }
		public IrrelevatDateTimeException(string message, Exception inner) : base(message, inner) { }
	}
}
