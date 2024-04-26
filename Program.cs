using System;
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
			var branch = args.Arg("branch");
			var noCredit = args.ArgBool("nocredit");
			var untagged = args.ArgBool("untagged");
			var showVersion = args.ArgBool("version");

			var outFile = args.Arg("output");

			if(args.Length == 0)
			{
				ShowUsage();
				return;
			}

			// validate
			if (!args.Check(out string message))
			{
				Console.WriteLine($"Error: {message}");
				ShowUsage();
				return;
			}

			void ShowUsage()
			{
				Console.WriteLine("Usage: ChangeLogFormatter --txt | --rtf | --md | --html [--nocredit] [--untagged] [--repo path] [--branch name] [--output filename]");
			}

			if (showVersion)
			{
				var name = Assembly.GetExecutingAssembly().GetName();
				Console.WriteLine($"{name.Name} {name.Version.Major}.{name.Version.Build}.{name.Version.Minor}.{name.Version.MinorRevision}");
				return;
			}

			if (type == GenerateReports.OutputType.None)
			{
				Console.WriteLine("Error: Specify file format --txt | --rtf | --md | --html");
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

					parser.Generate(repoPath, branch);
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
		private static readonly Dictionary<string, (bool Manditory, bool HasValue)> _known = new Dictionary<string, (bool, bool)>();

		// for	"-searchKey value"	return "value"
		internal static string Arg(this string[] args, string name, bool manditory = false)
		{
			_known.Add($"--{name}", (manditory, true));
			var index = Array.IndexOf(args, $"--{name}");
			return index != -1 && index + 1 < args.Length ? args[index + 1] : null;
		}

		// Simple boolean arg		"-someoption"	return true
		internal static bool ArgBool(this string[] args, string name, bool manditory = false)
		{
			_known.Add($"--{name}", (manditory, false));
			return args.Contains($"--{name}");
		}

		/// <summary>
		/// Check
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		public static bool Check(this string[] args, out string message)
		{
			var unknown = args.Where(x => x.StartsWith("-")).Except(_known.Keys);

			if (unknown.Any())
			{
				message = $"Unknown argument(s): {string.Join(", ", unknown)}";
				return false;
			}

			var missing = _known.Where(x => x.Value.Manditory).Select(x => x.Key).Except(args);

			if (missing.Any())
			{
				message = $"Missing argument(s): {string.Join(", ", missing)}";
				return false;
			}

			message = "";
			return true;
		}
	}
}
