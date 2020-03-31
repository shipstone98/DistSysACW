using System;
using System.Threading.Tasks;

namespace DistSysACWClient
{
	internal static class Program
	{
		private const String URI = "https://localhost:5001";

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
			bool exit = false;
			String input = Program.GetInput("Hello. What would you like to do?");
			Console.TreatControlCAsInput = false;
			Console.CancelKeyPress += new ConsoleCancelEventHandler((sender, e) => exit = true);

			using (Client client = new Client(Program.URI))
			{
				do
				{
					String[] split = input.Split(' ');

					try
					{
						switch (split[0].ToLower())
						{
							case "exit":
							case "quit":
								exit = true;
								break;
							case "protected":
								break;
							case "talkback":
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

					input = Program.GetInput("What would you like to do next?");
					Console.Clear();
				} while (!exit);
			}

			return 0;
		}
	}
}