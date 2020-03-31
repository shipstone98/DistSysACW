using System;

namespace DistSysACWClient
{
	public class ApiKeyNotSetException: Exception
	{
		public ApiKeyNotSetException(): this("The client instance's API key was not set.") { }
		public ApiKeyNotSetException(String message): base(message) { }
		public ApiKeyNotSetException(String message, Exception inner) : base(message, inner) { }
	}
}
