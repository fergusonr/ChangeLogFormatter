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

			if(args.Contains("-html"))
				type = Parser.OutputType.Html;
			if(args.Contains("-md"))
				type = Parser.OutputType.Markdown;
			if (args.Contains("-text"))
				type = Parser.OutputType.Text;

			if(type == Parser.OutputType.None)
			{
				Console.WriteLine("Usage: ChangeLogFormatter -text|-md|-html [outfile]");
				return;
			}

			var outFile = args.Where(x => !x.StartsWith("-"));

			using (TextWriter outStream = outFile.Any() ? new StreamWriter(outFile.Last()) : Console.Out)
			{
				var parser = new Parser(type, outStream);

				if (!parser.Parse())
					Console.Error.WriteLine("Error: No tags found");
			}
		}
	}
}
