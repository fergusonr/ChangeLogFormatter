using System;
using System.IO;
using System.Linq;

namespace ChangeLogFormatter
{
	public static class Program
	{
		static void Main(string[] args)
		{
			var type = Parser.OutputType.None;

			if (args.ArgBool("html"))
				type = Parser.OutputType.Html;
			if (args.ArgBool("rtf"))
				type = Parser.OutputType.Rtf;
			if (args.ArgBool("md"))
				type = Parser.OutputType.Markdown;
			if (args.ArgBool("text"))
				type = Parser.OutputType.Text;

			var repoPath = args.Arg("repo") ?? ".";

			if (type == Parser.OutputType.None)
			{
				Console.WriteLine("Usage: ChangeLogFormatter -text | -rtf | -md | -html [-repo path] [outfile]");
				return;
			}

			var outFile = args.Where(x => !x.StartsWith("-") && x != repoPath);

			using (TextWriter outStream = outFile.Any() ? new StreamWriter(outFile.Last()) : Console.Out)
			{
				var parser = new Parser(type, outStream);

				if (!parser.Parse(repoPath))
					Console.Error.WriteLine("Error: No tags found");
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
