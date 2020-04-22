using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using CoreExtensions;

namespace DistSysACWClient
{
	internal static class Program
	{
		private const String DefaultURI = "https://localhost:5001";
		//private const String DefaultURI = "http://distsysacw.azurewebsites.net/3978094";
		private const String UserSetupMessage = "You need to do a User Post or User Set first";
		private const String WaitingMessage = "...please wait...";

		private static String ApiKey = "f07c0e68-5069-4705-9060-59b80a62baae";
		private static String PublicKey = null;		
		private static String UserName = "BrianKernighan";

		private static String GetInput() => Program.GetInput(null);

		private static String GetInput(String message)
		{
			Console.Write(message ?? "");
			String input;

			do
			{
				input = Console.ReadLine();
			} while (input.Trim().Length == 0);

			return input;
		}

		private static async Task<int> Main(String[] args)
		{
			String input = Program.GetInput("Hello. What would you like to do? ");

			using (Client client = new Client(Program.DefaultURI))
			{
				while (true)
				{
					String[] split = input.Split(' ');

					try
					{
						switch (split[0].ToLower())
						{
							case "connect":
							{
								Task<Exception> task = client.ConnectAsync();
								Console.WriteLine(Program.WaitingMessage);
								Exception taskException = await task;
								Console.WriteLine(taskException is null ? "Connected successfully" : taskException.ToString());
								break;
							}
							
							case "exit":
							case "quit":
								goto break_loop;

							case "protected":
								switch (split[1].ToLower())
								{
									case "addfifty":
										if (split.Length > 3 || !Int64.TryParse(split[2], out long result))
										{
											throw new IndexOutOfRangeException();
										}

										if (Program.ApiKey is null)
										{
											Console.WriteLine(Program.UserSetupMessage);
										}

										else if (Program.PublicKey is null)
										{
											Console.WriteLine("Client doesn't yet have the public key");
										}

										else
										{
											Task<String> task = client.ProtectedAddFiftyAsync(Program.ApiKey, result, Program.PublicKey);
											Console.WriteLine(Program.WaitingMessage);
											Console.WriteLine(await task);
										}
										
										break;

									case "get":
										if (split[2].ToLower() != "publickey" || split.Length > 3)
										{
											throw new IndexOutOfRangeException();
										}

										if (Program.ApiKey is null)
										{
											Console.WriteLine(Program.UserSetupMessage);
										}

										else
										{
											Task<String> task = client.ProtectedGetPublicKeyAsync(Program.ApiKey);
											Console.WriteLine(Program.WaitingMessage);
											String publicKey = await task;

											if (publicKey is null)
											{
												Console.WriteLine("Couldn't get the Public Key");
											}

											else
											{
												Program.PublicKey = publicKey;
												Console.WriteLine("Got Public Key");
											}
										}

										break;

									case "hello":
										if (Program.ApiKey is null)
										{
											Console.WriteLine(Program.UserSetupMessage);
										}

										else
										{
											Task<String> task = client.ProtectedHelloAsync(Program.ApiKey);
											Console.WriteLine(Program.WaitingMessage);
											Console.WriteLine(await task);
										}

										break;

									case "sha1":
										if (Program.ApiKey is null)
										{
											Console.WriteLine(Program.UserSetupMessage);
										}

										else
										{
											if (split.Length == 2)
											{
												throw new IndexOutOfRangeException();
											}

											Task<String> task = client.ProtectedSha1Async(Program.ApiKey, String.Join(' ', split, 2, split.Length - 2));
											Console.WriteLine(Program.WaitingMessage);
											Console.WriteLine(await task);
										}

										break;

									case "sha256":
										if (Program.ApiKey is null)
										{
											Console.WriteLine(Program.UserSetupMessage);
										}

										else
										{
											if (split.Length == 2)
											{
												throw new IndexOutOfRangeException();
											}
											
											Task<String> task = client.ProtectedSha256Async(Program.ApiKey, String.Join(' ', split, 2, split.Length - 2));
											Console.WriteLine(Program.WaitingMessage);
											Console.WriteLine(await task);
										}

										break;

									case "sign":
										if (Program.ApiKey is null)
										{
											Console.WriteLine(Program.UserSetupMessage);
										}

										else if (Program.PublicKey is null)
										{
											Console.WriteLine("Client doesn't yet have the public key");
										}

										else
										{
											if (split.Length == 2)
											{
												throw new IndexOutOfRangeException();
											}
											
											String message = String.Join(' ', split, 2, split.Length - 2);
											Task<String> task = client.ProtectedSignAsync(Program.ApiKey, message);
											Console.WriteLine(Program.WaitingMessage);

											CspParameters parameters = new CspParameters()
											{
												Flags = CspProviderFlags.UseMachineKeyStore
											};

											RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(parameters);
											rsa.FromXmlStringCore22(Program.PublicKey);
											byte[] originalBytes = Encoding.ASCII.GetBytes(message);
											String messageWithoutDashes = (await task).Replace("-", "");
											byte[] signedBytes = new byte[messageWithoutDashes.Length / 2];

											for (int i = 0; i < signedBytes.Length; i ++)
											{
												signedBytes[i] = Convert.ToByte(messageWithoutDashes.Substring(i * 2, 2), 16);
											}

											bool signed = rsa.VerifyData(originalBytes, signedBytes, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
											Console.WriteLine(signed ? "Message was successfully signed" : "Message was not successfully signed");
										}

										break;

									default:
										throw new IndexOutOfRangeException();
								}

								break;

							case "talkback":
								switch (split[1].ToLower())
								{
									case "hello":
									{
										Task<String> task = client.TalkBackHelloAsync();
										Console.WriteLine(Program.WaitingMessage);
										Console.WriteLine(await task);
										break;
									}

									case "sort":
									{
										Task<int[]> task;

										if (split[2].StartsWith("["))
										{
											task = client.TalkBackSortAsync(String.Join(' ', split, 2, split.Length - 2));
										}

										else
										{
											int[] arr = new int[split.Length - 2];

											for (int i = 2; i < split.Length; i++)
											{
												arr[i] = Int32.Parse(split[i]);
											}

											task = client.TalkBackSortAsync(arr);
										}

										Console.WriteLine(Program.WaitingMessage);
										int[] sortedArray = await task;

										if (sortedArray is null)
										{
											throw new IndexOutOfRangeException();
										}

										StringBuilder sb = new StringBuilder();
										sb.Append('[');

										foreach (int item in sortedArray)
										{
											sb.Append(item + ",");
										}

										sb.Remove(sb.Length - 1, 1);
										sb.Append(']');
										Console.WriteLine(sb.ToString());

										break;
									}

									default:
										throw new IndexOutOfRangeException();
								}

								break;

							case "user":
								switch (split[1].ToLower())
								{
									case "delete":
									{
										if (Program.ApiKey is null || Program.UserName is null)
										{
											Console.WriteLine(Program.UserSetupMessage);
										}

										else
										{
											Task<bool> responseTask = client.UserDeleteAsync(Program.ApiKey, Program.UserName);
											Console.WriteLine(Program.WaitingMessage);
											bool response = await responseTask;
											Console.WriteLine(response ? "True" : "False");
										}

										break;
									}

									case "get":
									{
										Task<String> responseTask = client.UserGetAsync(split[2]);
										Console.WriteLine(Program.WaitingMessage);
										String response = await responseTask;

										if (response is null)
										{
											throw new IndexOutOfRangeException();
										}

										Console.WriteLine(response);
										break;
									}

									case "post":
									{
										if (split.Length > 3)
										{
											throw new IndexOutOfRangeException();
										}

										Task<String> responseTask = client.UserPostAsync(split[2]);
										Console.WriteLine(Program.WaitingMessage);
										String response = await responseTask;

										if (response.StartsWith("Oops"))
										{
											Console.WriteLine(response);
										}

										else
										{
											Program.ApiKey = response;
											Program.UserName = split[2];
											Console.WriteLine("Got API Key");
										}

										break;
									}

									case "role":
									{
										if (Program.ApiKey is null)
										{
											Console.WriteLine(Program.UserSetupMessage);
										}

										else
										{
											if (split.Length > 4)
											{
												throw new IndexOutOfRangeException();
											}

											String role;

											switch (split[3].ToLower())
											{
												case "admin":
													role = "Admin";
													break;
												case "user":
													role = "User";
													break;
												default:
													throw new IndexOutOfRangeException();
											}

											Task<String> responseTask = client.UserChangeRoleAsync(Program.ApiKey, split[2], role);
											Console.WriteLine(Program.WaitingMessage);
											Console.WriteLine(await responseTask);
										}

										break;
									}

									case "set":
										if (split.Length > 4)
										{
											throw new IndexOutOfRangeException();
										}

										Program.UserName = split[2];
										Program.ApiKey = split[3];
										Console.WriteLine("Stored");
										break;

									default:
										throw new IndexOutOfRangeException();
								}

								break;
							default:
								throw new IndexOutOfRangeException();
						}
					}

					catch (IndexOutOfRangeException)
					{
						Console.WriteLine("ERROR: invalid input");
					}

					input = Program.GetInput("What would you like to do next? ");
					Console.Clear();
				}
			}

		break_loop:
			return 0;
		}
	}
}