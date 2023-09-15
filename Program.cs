using System;

namespace ChangeLogFormatter
{
	public static class Program
	{
		static void Main(string[] args)
		{
			#region commandline args
			var type = GenerateReports.OutputType.None;

			if (args.ArgBool("html"))
				type = GenerateReports.OutputType.Html;
			if (args.ArgBool("rtf"))
				type = GenerateReports.OutputType.Rtf;
			if (args.ArgBool("md"))
				type = GenerateReports.OutputType.Markdown;
			if (args.ArgBool("text"))
				type = GenerateReports.OutputType.Text;

			var repoPath = args.Arg("repo") ?? ".";
			var outFile = args.Where(x => !x.StartsWith("-") && x != repoPath);

			var noCredit = args.ArgBool("nocredit");
			var untagged = args.ArgBool("untagged");

			var unknown = args.Unknown();

			if(args.Length == 0 || unknown.Any())
			{
				if(unknown.Any())
					Console.WriteLine($"Error: Unknown argument: {string.Join(',', unknown)}");

				Console.WriteLine("Usage: ChangeLogFormatter -text | -rtf | -md | -html [-nocredit] [-untagged] [-repo path] [outfile]");
				return;
			}
			#endregion

			try
			{
				using (TextWriter outStream = outFile.Any() ? new StreamWriter(outFile.Last()) : Console.Out)
				{
					var parser = new GenerateReports(type, outStream) 
					{
						NoCredit = noCredit,
						Untagged = untagged
					};

					parser.Generate(repoPath);
				}
			}
			catch (Exception e)
			{
				Console.Error.WriteLine($"Error: {e.Message}");
			}
		}
	}

	internal static class ArgsExtensions
	{
		static List<string> _known = new List<string>();

		// for	"-searchKey value"	return "value"
		internal static string Arg(this string[] args, string name)
		{
			_known.Add($"-{name}");
			var index = Array.IndexOf(args, $"-{name}");
			return index != -1 && index + 1 < args.Length ? args[index + 1] : null;
		}

		// Simple boolean arg		"-someoption"	return true
		internal static bool ArgBool(this string[] args, string name)
		{
			_known.Add($"-{name}");
			return args.Contains($"-{name}");
		}

		internal static IEnumerable<string> Unknown(this string[] args)
		{
			return args.Where(x => x.StartsWith("-")).Except(_known);
		}
	}
}
