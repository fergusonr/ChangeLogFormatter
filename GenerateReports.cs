using System;
using System.Drawing;

#if !NET5_0_OR_GREATER
using System.IO;
using System.Linq;
using System.Collections.Generic;
#endif

using LibGit2Sharp;

namespace ChangeLogFormatter
{
	public class GenerateReports
	{
		public enum OutputType { None, Text, Rtf, Html, Markdown };

		public bool NoCredit { get; set; }
		public bool Untagged { get; set; }

		private struct Tag
		{
			internal string Name;
			internal DateTime Date;
		}

		private readonly Dictionary<Tag, List<string>> _data = new Dictionary<Tag, List<string>>();
		private readonly OutputType _outputType;
		private readonly TextWriter _outStream;
		private readonly bool _stdout;
		private const string _untagged = "Untagged";

		/// <summary>
		/// Git log parser
		/// </summary>
		/// <param name="type"></param>
		public GenerateReports(OutputType type, TextWriter outStream)
		{
			_outputType = type;
			_outStream = outStream;
			_stdout = outStream == Console.Out;
		}

		/// <summary>
		/// Format git log
		/// </summary>
		public bool Generate(string repoPath)
		{
			var repo = new Repository(repoPath);

			var currentTag = new Tag();

			foreach (var commit in repo.Commits.OrderByDescending(x => x.Author.When))
			{
				var tag = repo.Tags.FirstOrDefault(x => commit.Sha == x.Reference.TargetIdentifier);

				if (tag != null)
				{
					currentTag.Name = tag.FriendlyName;
					currentTag.Date = commit.Author.When.Date;
				}

				if (currentTag.Date == DateTime.MinValue)
				{
					if (Untagged)
					{
						currentTag.Name = _untagged;
						currentTag.Date = commit.Author.When.Date;
					}
					else
						continue;// skip untagged commits
				}

				if (!_data.ContainsKey(currentTag))
					_data[currentTag] = new List<string>();

				_data[currentTag].Add(commit.Message);
			}

			repo.Dispose();

			if (_data.Count == 0)
				throw new Exception("No tags found");

			///
			/// Generate log report
			///
			const string text = "Generated with";
			const string gitUrl = "https://github.com/fergusonr/ChangeLogFormatter";

			// tag version banner
			var bCol = Color.DarkGreen;
			var bColU = Color.Orange;
			var fCol = Color.White;

			///
			/// Html
			///
			if (_outputType == OutputType.Html)
			{
				_outStream.WriteLine("<html>\n<body>");

				foreach (var tag in _data.OrderByDescending(x => x.Key.Date))
				{
					var col = tag.Key.Name == _untagged ? bColU : bCol;

					_outStream.WriteLine($"<b style=\"background-color:rgb({col.R},{col.G},{col.B});color:rgb({fCol.R},{fCol.G},{fCol.B})\">&nbsp;{tag.Key.Name}&nbsp;</b>");
					_outStream.WriteLine($"<table>\n<tr><td><b>{tag.Key.Date.ToLongDateString()}</b></td></tr>");

					foreach (var message in tag.Value)
						_outStream.WriteLine($"<tr><td>&nbsp;&nbsp;{message}</td></tr>");

					_outStream.WriteLine("</table>\n<br>");
				}

				if (!NoCredit)
					_outStream.WriteLine($@"{text}: <a href=""{gitUrl}"">{gitUrl}</a>");

				_outStream.WriteLine("</body>\n</html>");
			}

			///
			/// Markdown
			///
			else if (_outputType == OutputType.Markdown)
			{
				foreach (var tag in _data.OrderByDescending(x => x.Key.Date))
				{
					var col = tag.Key.Name == _untagged ? bColU : bCol;

					_outStream.WriteLine($"#### <span style=\"background-color:rgb({col.R},{col.G},{col.B});color:rgb({fCol.R},{fCol.G},{fCol.B})\">{tag.Key.Name}</span>\n**{tag.Key.Date.ToLongDateString()}**");

					foreach (var message in tag.Value)
						_outStream.WriteLine($"- {message}");
				}

				_outStream.WriteLine();

				if (!NoCredit)
					_outStream.WriteLine($"{text}: [{gitUrl}]({gitUrl})");
			}

			///
			/// Rtf
			///
			else if (_outputType == OutputType.Rtf)
			{
				_outStream.WriteLine(@"{\rtf1\ansi{\fonttbl\f0\fCourier New;}");
				_outStream.WriteLine($@"{{\colortbl;\red{fCol.R}\green{fCol.G}\blue{fCol.B};\red{bCol.R}\green{bCol.G}\blue{bCol.B};\red{bColU.R}\green{bColU.G}\blue{bColU.B};}}");

				foreach (var tag in _data.OrderByDescending(x => x.Key.Date))
				{
					var col = tag.Key.Name == _untagged ? 3 : 2;

					_outStream.WriteLine($@"{{\pard\li0\highlight1\cf1\highlight{col}\b1  {tag.Key.Name} }}\line\b1 {tag.Key.Date.ToLongDateString()}\b0\par");

					_outStream.WriteLine(@"{\pard\li400");

					foreach (var message in tag.Value)
					{
						var messageMod = message.Count(x => x == '\n') > 1 ? message.Replace("\n", "\\line\n") : message;
						_outStream.WriteLine($@"\bullet  {messageMod}\line");
					}

					_outStream.WriteLine(@"\par}");
				}

				if(!NoCredit)
					_outStream.WriteLine($@"\fs20{text}: {{\field{{\*\fldinst HYPERLINK ""{gitUrl}""}}}}\line");

				_outStream.WriteLine("}");
			}

			///
			/// Text
			///
			else
			{
				foreach (var tag in _data.OrderByDescending(x => x.Key.Date))
				{
					if (_stdout)
					{
						Console.BackgroundColor = tag.Key.Name == _untagged ? ConsoleColor.DarkYellow : ConsoleColor.DarkGreen;
						Console.ForegroundColor = ConsoleColor.White;
					}

					_outStream.Write($" {tag.Key.Name} ");

					// https://stackoverflow.com/questions/31140768/console-resetcolor-is-not-resetting-the-line-after-completely
					if (_stdout)
						Console.ResetColor();
					_outStream.WriteLine();

					if (_stdout)
					{
						Console.BackgroundColor = ConsoleColor.DarkGray;
						Console.ForegroundColor = ConsoleColor.White;
					}

					_outStream.Write($" {tag.Key.Date.ToLongDateString()} ");

					if (_stdout)
						Console.ResetColor();
					_outStream.WriteLine();

					foreach (var message in tag.Value)
						_outStream.WriteLine($"  {message}");
					_outStream.WriteLine();
				}

				if (!NoCredit)
				{
					_outStream.WriteLine();
					_outStream.WriteLine($"{text}: {gitUrl}");
				}
			}

			_data.Clear();

			return true;
		}
	}
}
