using System;
using System.Drawing;

using LibGit2Sharp;

namespace ChangeLogFormatter
{
	public class GenerateReports
	{
		public enum OutputType { None, Text, Rtf, Html, Markdown };

		private struct Tag
		{
			internal string Name;
			internal DateTime Date;
		}

		private readonly Dictionary<Tag, List<string>> _data = new Dictionary<Tag, List<string>>();
		private readonly OutputType _outputType;
		private readonly TextWriter _outStream;
		private readonly bool _noCredit;

		/// <summary>
		/// Git log parser
		/// </summary>
		/// <param name="type"></param>
		public GenerateReports(OutputType type, TextWriter outStream, bool noCredit)
		{
			_outputType = type;
			_outStream = outStream;
			_noCredit = noCredit;
		}

		/// <summary>
		/// Format git log
		/// </summary>
		public bool Generate(string repoPath)
		{
			Repository repo;

			try
			{
				repo = new Repository(repoPath);

				var currentTag = new Tag();

				foreach (var commit in repo.Commits.OrderByDescending(x => x.Author.When))
				{
					var tag = repo.Tags.FirstOrDefault(x => commit.Sha == x.Reference.TargetIdentifier);

					if (tag != null)
					{
						currentTag.Name = tag.FriendlyName;
						currentTag.Date = commit.Author.When.Date;
					}

					if (currentTag.Date == DateTime.MinValue) // skip untagged commits
						continue;

					if (!_data.ContainsKey(currentTag))
						_data[currentTag] = new List<string>();

					_data[currentTag].Add(commit.MessageShort);
				}

				repo.Dispose();

				if (_data.Count == 0)
					return false; // no tags found

				///
				/// Generate log report
				///
				const string text = "Generated with";
				const string gitUrl = "https://github.com/fergusonr/ChangeLogFormatter";

				// tag version banner
				var bCol = Color.DarkGreen;
				var fCol = Color.White;

				///
				/// Html
				///
				if (_outputType == OutputType.Html)
				{
					_outStream.WriteLine("<html>\n<body>");

					foreach (var tag in _data.OrderByDescending(x => x.Key.Date))
					{
						_outStream.WriteLine($"<b style=\"background-color:rgb({bCol.R},{bCol.G},{bCol.B});color:rgb({fCol.R},{fCol.G},{fCol.B})\">&nbsp;{tag.Key.Name}&nbsp;</b>");
						_outStream.WriteLine($"<table>\n<tr><td><b>{tag.Key.Date.ToLongDateString()}</b></td></tr>");

						foreach (var message in tag.Value)
							_outStream.WriteLine($"<tr><td>&nbsp;&nbsp;{message}</td></tr>");

						_outStream.WriteLine("</table>\n<br>");
					}

					if (!_noCredit)
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
						_outStream.WriteLine($"#### <span style=\"background-color:rgb({bCol.R},{bCol.G},{bCol.B});color:rgb({fCol.R},{fCol.G},{fCol.B})\">{tag.Key.Name}</span>\n{tag.Key.Date.ToLongDateString()}");

						foreach (var message in tag.Value)
							_outStream.WriteLine($"- {message}");
					}

					_outStream.WriteLine();

					if (!_noCredit)
						_outStream.WriteLine($"{text}: [{gitUrl}]({gitUrl})");
				}

				///
				/// Rtf
				///
				else if (_outputType == OutputType.Rtf)
				{
					_outStream.WriteLine(@"{\rtf1\ansi{\fonttbl\f0\fCourier New;}");
					_outStream.WriteLine($@"{{\colortbl;\red{fCol.R}\green{fCol.G}\blue{fCol.B};\red{bCol.R}\green{bCol.G}\blue{bCol.B};}}");

					foreach (var tag in _data.OrderByDescending(x => x.Key.Date))
					{
						_outStream.WriteLine($@"{{\pard\li0\highlight1\cf1\highlight2\b1  {tag.Key.Name} }}\line\b1 {tag.Key.Date.ToLongDateString()}\b0\par");

						_outStream.WriteLine(@"{\pard\li400");

						foreach (var message in tag.Value)
							_outStream.WriteLine($@"\bullet  {message}\line");

						_outStream.WriteLine(@"\par}");
					}

					if(!_noCredit)
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
						_outStream.WriteLine($"{tag.Key.Name} {tag.Key.Date.ToLongDateString()}");

						foreach (var message in tag.Value)
							_outStream.WriteLine($"  {message}");
						_outStream.WriteLine();
					}

					if (!_noCredit)
					{
						_outStream.WriteLine();
						_outStream.WriteLine($"{text}: {gitUrl}");
					}
				}

				_data.Clear();
			}
			catch (Exception e)
			{
				Console.Error.WriteLine(e.Message);
				return false;
			}

			return true;
		}
	}
}
