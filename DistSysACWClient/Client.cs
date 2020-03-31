using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DistSysACWClient
{
	public class Client: IDisposable
	{
		private String _ApiKey;
		private String _UserName;

		public String ApiKey
		{
			get => this._ApiKey;

			set
			{
				if (value is null)
				{
					throw new ArgumentNullException(nameof (value));
				}

				if (String.IsNullOrWhiteSpace(value))
				{
					throw new ArgumentException(nameof (value));
				}

				this._ApiKey = value;
			}
		}

		private bool AreUnmanagedDisposed { get; set; }
		private HttpClient HttpClient { get; }

		public String UserName
		{
			get => this._UserName;

			set
			{
				if (value is null)
				{
					throw new ArgumentNullException(nameof (value));
				}

				if (String.IsNullOrWhiteSpace(value))
				{
					throw new ArgumentException(nameof (value));
				}

				this._UserName = value;
			}
		}

		private Client()
		{
			this._ApiKey = this._UserName = null;
			this.AreUnmanagedDisposed = false;
			this.HttpClient = new HttpClient();
		}

		public Client(String uri): this()
		{
			if (uri is null)
			{
				throw new ArgumentNullException(nameof (uri));
			}

			if (String.IsNullOrWhiteSpace(uri))
			{
				throw new ArgumentException(nameof (uri));
			}

			try
			{
				this.HttpClient.BaseAddress = new Uri(uri);
			}

			catch (UriFormatException)
			{
				throw;
			}
		}

		public Client(String uri, String apiKey, String userName): this(uri)
		{
			if (apiKey is null)
			{
				throw new ArgumentNullException(nameof (apiKey));
			}

			if (String.IsNullOrWhiteSpace(apiKey))
			{
				throw new ArgumentException(nameof (apiKey));
			}

			if (userName is null)
			{
				throw new ArgumentNullException(nameof (userName));
			}

			if (String.IsNullOrWhiteSpace(userName))
			{
				throw new ArgumentException(nameof (userName));
			}

			this._ApiKey = apiKey;
			this._UserName = userName;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposeManaged)
		{
			if (this.AreUnmanagedDisposed)
			{
				this.HttpClient.Dispose();
				this.AreUnmanagedDisposed = true;
			}
		}
	}
}
