using System;
using System.Threading.Tasks;

namespace DistSysACWClient
{
	internal static class Program
	{
		private const String URI = "https://localhost:44307";
		private const String WaitingMessage = "...please wait...";

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
										Task<String> task = client.GetTalkBackHelloAsync();
										Console.WriteLine(Program.WaitingMessage);
										Console.WriteLine(await task);
										break;
									case "split":
										break;
									default:
										throw new IndexOutOfRangeException();
								}

								break;

							case "user":
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