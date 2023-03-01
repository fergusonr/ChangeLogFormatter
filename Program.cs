using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace ChangeLogFormatter
{
	//
	// git log --pretty=format:"%cd %d %s" --date=format:"%d/%m/%y" | changeLogFormatter       > changelog.txt
	// git log --pretty=format:"%cd %d %s" --date=format:"%d/%m/%y" | changeLogFormatter -md   > changelog.md
	// git log --pretty=format:"%cd %d %s" --date=format:"%d/%m/%y" | changeLogFormatter -html > changelog.html
	//

	public static class Program
	{
		static void Main(string[] args)
		{
			var type = Parser.OutputType.Text;

			if(args.Contains("-html"))
				type = Parser.OutputType.Html;
			if(args.Contains("-md"))
				type = Parser.OutputType.Markdown;

			var parser = new Parser(type);

			var fileArgs = args.Where(x => !x.StartsWith("-"));

			if(fileArgs.Any() && File.Exists(fileArgs.First()))
			{
				var outStream = fileArgs.Count() == 2 ? new StreamWriter(fileArgs.Last()) : Console.Out;
				parser.Parse(File.ReadAllLines(fileArgs.First()), outStream);
				outStream.Close();
			}
			else
			{
				var input = new List<string>();

				string line;
				while ((line = Console.ReadLine()) != null)
					input.Add(line);

				parser.Parse(input.ToArray(), Console.Out);
			}
		}
	}
}
