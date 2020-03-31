using System;

namespace DistSysACWClient
{
	public class UserNameNotSetException: Exception
	{
		public UserNameNotSetException() : this("The client instance's user name was not set.") { }
		public UserNameNotSetException(String message) : base(message) { }
		public UserNameNotSetException(String message, Exception inner) : base(message, inner) { }
	}
}
