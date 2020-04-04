using System;
using System.Text;
using System.Threading.Tasks;

namespace DistSysACWClient
{
	internal static class Program
	{
		private const String URI = "https://localhost:5001";
		private const String WaitingMessage = "...please wait...";

		private static String ApiKey = null;
		private static String UserName = null;

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

			using (Client client = new Client(Program.URI))
			{
				while (true)
				{
					String[] split = input.Split(' ');

					try
					{
						switch (split[0].ToLower())
						{
							case "exit":
							case "quit":
								goto break_loop;
							case "protected":
								break;

							case "talkback":
								switch (split[1].ToLower())
								{
									case "hello":
									{
										Task<String> task = client.GetTalkBackHelloAsync();
										Console.WriteLine(Program.WaitingMessage);
										Console.WriteLine(await task);
										break;
									}

									case "sort":
									{
										Task<int[]> task;

										if (split[2].StartsWith("["))
										{
											task = client.GetTalkBackSortAsync(String.Join(' ', split, 2, split.Length - 2));
										}

										else
										{
											int[] arr = new int[split.Length - 2];

											for (int i = 2; i < split.Length; i++)
											{
												arr[i] = Int32.Parse(split[i]);
											}

											task = client.GetTalkBackSortAsync(arr);
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