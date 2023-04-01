using System;

namespace ChangeLogFormatter
{
	public static class Program
	{
		static void Main(string[] args)
		{
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

			if (type == GenerateReports.OutputType.None)
			{
				Console.WriteLine("Usage: ChangeLogFormatter -text | -rtf | -md | -html [-nocredit] [-repo path] [outfile]");
				return;
			}

			var outFile = args.Where(x => !x.StartsWith("-") && x != repoPath);

			using (TextWriter outStream = outFile.Any() ? new StreamWriter(outFile.Last()) : Console.Out)
			{
				var parser = new GenerateReports(type, outStream, args.ArgBool("nocredit"));

				try
				{
					parser.Generate(repoPath);
				}
				catch(Exception e)
				{
					Console.Error.WriteLine($"Error: {e.Message}");
				}
			}
		}
	}

	internal static class ArgsExtensions
	{
		// for	"-searchKey value"	return "value"
		internal static string Arg(this string[] args, string name)
		{
			var index = Array.IndexOf(args, $"-{name}");
			return index != -1 && index + 1 < args.Length ? args[index + 1] : null;
		}

		// Simple boolean arg		"-someoption"	return true
		internal static bool ArgBool(this string[] args, string name)
		{
			return args.Contains($"-{name}");
		}
	}
}
