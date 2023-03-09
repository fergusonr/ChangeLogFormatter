using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace ChangeLogFormatter
{
	//
	// git log --pretty=format:"%cd %d %s" --date=format:"%d/%m/%y" | changeLogFormatter -text > changelog.txt
	// git log --pretty=format:"%cd %d %s" --date=format:"%d/%m/%y" | changeLogFormatter -md   > changelog.md
	// git log --pretty=format:"%cd %d %s" --date=format:"%d/%m/%y" | changeLogFormatter -html > changelog.html
	//

	public static class Program
	{
		static void Main(string[] args)
		{
			var type = Parser.OutputType.None;

			if(args.Contains("-html"))
				type = Parser.OutputType.Html;
			if(args.Contains("-md"))
				type = Parser.OutputType.Markdown;
			if (args.Contains("-text"))
				type = Parser.OutputType.Text;

			if(type == Parser.OutputType.None)
			{
				Console.WriteLine("Usage: ChangeLogFormatter -text|-md|-html [infile] [outfile]");
				Console.WriteLine();
				Console.WriteLine(@"git log --pretty=format:""%cd %d %s"" --date=format:""%d/%m/%y"" | changeLogFormatter -md > changelog.md");
				return;
			}

			var parser = new Parser(type);

			var fileArgs = args.Where(x => !x.StartsWith("-"));

			if(fileArgs.Any() && File.Exists(fileArgs.First()))
			{
				using (TextWriter outStream = fileArgs.Count() == 2 ? new StreamWriter(fileArgs.Last()) : Console.Out)
				{ 
					if (!parser.Parse(File.ReadAllLines(fileArgs.First()), outStream))
						Console.Error.WriteLine("Error: No tags found");
				}
			}
			else
			{
				var input = new List<string>();

				string line;
				while ((line = Console.ReadLine()) != null)
					input.Add(line);

				if(!parser.Parse(input.ToArray(), Console.Out))
					Console.Error.WriteLine("Error: No tags found");
			}
		}
	}
}
