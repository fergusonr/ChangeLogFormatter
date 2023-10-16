#if !NET5_0_OR_GREATER
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
#endif

using System.Reflection;

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
				type = GenerateReports.OutputType.Md;
			if (args.ArgBool("txt"))
				type = GenerateReports.OutputType.Txt;

			var repoPath = args.Arg("repo") ?? ".";
			var outFile = args.FirstOrDefault(x => !x.StartsWith("-") && x != repoPath);

			var noCredit = args.ArgBool("nocredit");
			var untagged = args.ArgBool("untagged");
			var showVersion = args.ArgBool("version");

			// validate
			var unknown = args.Unknown();

			if (args.Length == 0 || unknown.Any())
			{
				if(unknown.Any())
					Console.WriteLine($"Error: Unknown argument: {string.Join(",", unknown)}");

				Console.WriteLine("Usage: ChangeLogFormatter -txt | -rtf | -md | -html [-nocredit] [-untagged] [-repo path] [outfile]");
				return;
			}

			if (showVersion)
			{
				var name = Assembly.GetExecutingAssembly().GetName();

				var version = name.Version;
				Console.WriteLine($"{name.Name} {version.Major}.{version.Build}.{version.Minor}.{version.MinorRevision}");
				return;
			}

			if (type == GenerateReports.OutputType.None)
			{
				Console.WriteLine("Error: Specify file format -txt | -rtf | -md | -html");
				return;
			}

			if(outFile != null)
			{
				var ext = Path.GetExtension(outFile);

				if (ext == string.Empty)
				{
					outFile += "." + type.ToString().ToLower();
					Console.WriteLine($"Outfile: {outFile}");
				}
				else if(!ext.Substring(1).Equals(type.ToString(), StringComparison.InvariantCultureIgnoreCase))
				{
					Console.WriteLine($"Invalid extension {ext}");
					return;
				}
			}
			#endregion

			try
			{
				using (TextWriter outStream = outFile != null ? new StreamWriter(outFile) : Console.Out)
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
