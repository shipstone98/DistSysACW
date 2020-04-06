using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Newtonsoft.Json;

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

		public async Task<Exception> ConnectAsync()
		{
			try
			{
				HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Options, this.HttpClient.BaseAddress.AbsoluteUri);
				await this.HttpClient.SendAsync(request);
				return null;
			}

			catch (Exception ex)
			{
				return ex;
			}
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

		public async Task<String> ProtectedGetPublicKeyAsync(String apiKey)
		{
			if (apiKey is null)
			{
				throw new ArgumentNullException(nameof (apiKey));
			}

			if (String.IsNullOrWhiteSpace(apiKey))
			{
				throw new ArgumentException(nameof (apiKey));
			}

			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{this.HttpClient.BaseAddress.AbsoluteUri}api/protected/getpublickey");
			request.Headers.Add("ApiKey", apiKey);
			HttpResponseMessage response = await this.HttpClient.SendAsync(request);
			return await response.Content.ReadAsStringAsync();
		}

		public async Task<String> ProtectedHelloAsync(String apiKey)
		{
			if (apiKey is null)
			{
				throw new ArgumentNullException(nameof (apiKey));
			}

			if (String.IsNullOrWhiteSpace(apiKey))
			{
				throw new ArgumentException(nameof (apiKey));
			}

			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{this.HttpClient.BaseAddress.AbsoluteUri}api/protected/hello");
			request.Headers.Add("ApiKey", apiKey);
			HttpResponseMessage response = await this.HttpClient.SendAsync(request);
			return await response.Content.ReadAsStringAsync();
		}

		private async Task<String> ProtectedMessageAsync(String apiKey, String message, String encryption)
		{
			if (apiKey is null)
			{
				throw new ArgumentNullException(nameof (apiKey));
			}

			if (String.IsNullOrWhiteSpace(apiKey))
			{
				throw new ArgumentException(nameof (apiKey));
			}

			if (message is null)
			{
				message = String.Empty;
			}

			String encodedMessage = HttpUtility.UrlEncode(message);
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{this.HttpClient.BaseAddress.AbsoluteUri}api/protected/{encryption}?message={encodedMessage}");
			request.Headers.Add("ApiKey", apiKey);
			HttpResponseMessage response = await this.HttpClient.SendAsync(request);
			return await response.Content.ReadAsStringAsync();
		}

		public async Task<String> ProtectedSha1Async(String apiKey, String message) => await this.ProtectedMessageAsync(apiKey, message, "sha1");
		public async Task<String> ProtectedSha256Async(String apiKey, String message) => await this.ProtectedMessageAsync(apiKey, message, "sha256");
		public async Task<String> ProtectedSignAsync(String apiKey, String message) => await this.ProtectedMessageAsync(apiKey, message, "sign");

		public async Task<String> GetTalkBackHelloAsync()
		{
			if (this.AreUnmanagedDisposed)
			{
				throw new ObjectDisposedException(nameof (this.HttpClient));
			}

			String uri = this.HttpClient.BaseAddress.AbsoluteUri;
			HttpResponseMessage response = await this.HttpClient.GetAsync(uri + "api/talkback/hello");
			return await response.Content.ReadAsStringAsync();
		}

		public async Task<int[]> GetTalkBackSortAsync(String arr)
		{
			if (arr is null)
			{
				throw new ArgumentNullException(nameof (arr));
			}

			if (String.IsNullOrWhiteSpace(arr))
			{
				throw new ArgumentException(nameof (arr));
			}

			String[] split = arr.Replace("[", "").Replace("]", "").Split(',');
			int[] input = new int[split.Length];

			try
			{
				for (int i = 0; i < split.Length; i ++)
				{
					input[i] = Int32.Parse(split[i]);
				}

				return await this.GetTalkBackSortAsync(input);
			}

			catch
			{
				return null;
			}
		}

		public async Task<int[]> GetTalkBackSortAsync(IEnumerable<int> arr)
		{
			if (this.AreUnmanagedDisposed)
			{
				throw new ObjectDisposedException(nameof (this.HttpClient));
			}

			if (arr is null)
			{
				throw new ArgumentNullException(nameof (arr));
			}

			StringBuilder sb = new StringBuilder();

			foreach (int item in arr)
			{
				sb.Append($"integers={item}&");
			}

			sb.Remove(sb.Length - 1, 1);
			String m = sb.ToString();
			HttpResponseMessage response = await this.HttpClient.GetAsync($"{this.HttpClient.BaseAddress.AbsoluteUri}api/talkback/sort?{sb}");

			try
			{
				String[] split = (await response.Content.ReadAsStringAsync()).Replace("[", "").Replace("]", "").Split(',');
				int[] output = new int[split.Length];

				for (int i = 0; i < split.Length; i ++)
				{
					output[i] = Int32.Parse(split[i]);
				}

				return output;
			}

			catch
			{
				return null;
			}
		}

		public async Task<String> UserChangeRoleAsync(String apiKey, String userName, String role)
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

			if (role is null)
			{
				throw new ArgumentNullException(nameof (role));
			}

			if (String.IsNullOrWhiteSpace(role))
			{
				throw new ArgumentException(nameof (role));
			}

			User user = new User
			{
				Role = role,
				UserName = userName
			};

			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{this.HttpClient.BaseAddress.AbsoluteUri}api/user/changerole");
			request.Headers.Add("ApiKey", apiKey);
			String userJson = JsonConvert.SerializeObject(user);
			request.Content = new StringContent(userJson, Encoding.UTF8, "application/json");
			HttpResponseMessage response = await this.HttpClient.SendAsync(request);
			return await response.Content.ReadAsStringAsync();
		}

		public async Task<bool> UserDeleteAsync(String apiKey, String userName)
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

			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, $"{this.HttpClient.BaseAddress.AbsoluteUri}api/user/removeuser?userName={userName}");
			request.Headers.Add("ApiKey", apiKey);
			HttpResponseMessage response = await this.HttpClient.SendAsync(request);
			return Boolean.TryParse(await response.Content.ReadAsStringAsync(), out bool result) ? result : false;
		}

		public async Task<String> UserGetAsync(String userName)
		{
			if (userName is null)
			{
				throw new ArgumentNullException(nameof (userName));
			}

			if (String.IsNullOrWhiteSpace(userName))
			{
				throw new ArgumentException(nameof (userName));
			}

			HttpResponseMessage response = await this.HttpClient.GetAsync($"{this.HttpClient.BaseAddress.AbsoluteUri}api/user/new?username={userName}");
			return await response.Content.ReadAsStringAsync();
		}

		public async Task<String> UserPostAsync(String userName)
		{
			if (userName is null)
			{
				throw new ArgumentNullException(nameof (userName));
			}

			if (String.IsNullOrWhiteSpace(userName))
			{
				throw new ArgumentException(nameof (userName));
			}

			User user = new User
			{
				UserName = userName
			};

			String userJson = JsonConvert.SerializeObject(user);
			StringContent content = new StringContent(userJson, Encoding.UTF8, "application/json");
			HttpResponseMessage response = await this.HttpClient.PostAsync($"{this.HttpClient.BaseAddress.AbsoluteUri}api/user/new", content);
			return await response.Content.ReadAsStringAsync();
		}
	}
}
