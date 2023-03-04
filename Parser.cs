using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ChangeLogFormatter
{
	public class Parser
	{
		public enum OutputType { Text, Html, Markdown };

		private readonly Dictionary<(string Tag, string Branch, DateTime Date), List<string>> _data = new Dictionary<(string, string, DateTime), List<string>>();
		private OutputType _outputType;

		public Parser(OutputType type)
		{
			_outputType = type;
		}

		/// <summary>
		/// Format incomming git log output
		/// </summary>
		/// <param name="lines"></param>
		/// <param name="outStream"></param>
		public bool Parse(string[] lines, TextWriter outStream)
		{
			/* Example log.........
			 
			  24/02/23	(HEAD -> master, tag: 1.0.0, origin/master, origin/HEAD) Merge ..\.\Windows\App\DepartureBoard
			  24/02/23	(HEAD -> master, tag: 1.0.0, origin/master) Support markdown
			  24/02/23  (HEAD -> master, tag: 1.0.0) Support marketdown
			  24/02/23  (HEAD -> master) /t:gitlog working
			  24/02/23  (origin/master) App token flavouring, git log from project msbuild task
			  21/02/23  (tag: 1.3.24) Function key shortcut combo fixed. NuGet package update
			  16/02/23  Fix and refactor menu helpers
			  18/10/22  (tag: v17.4.0-preview-22518-02) Final release branding for 17.4 (#8015)
			 */

			// Indispensable! --> https://regex101.com
			// https://docs.github.com/en/get-started/using-git/dealing-with-special-characters-in-branch-and-tag-names
			//
			var re = new Regex(@"^(\d+\/\d+\/\d+)\s+(?:\(([[\w\s\-\/>,])*tag:\s([\w\.\-_\/]+)(?:,\s)*(.*)\))*\s+(.*)");
			(string Tag, string Branch, DateTime Date) currentTag = ("", "", DateTime.MinValue);

			foreach (var line in lines)
			{
				var match = re.Match(line);

				if (match.Captures.Count == 0)
					continue;

				var date = DateTime.Parse(match.Groups[1].ToString());
				var tag = match.Groups[3].ToString();
				var branch= match.Groups[4].ToString();
				var message = match.Groups[5].ToString();

				if (tag != "")
					currentTag = (tag, branch, date);

				if (currentTag.Date == DateTime.MinValue)
					continue;

				if (!_data.ContainsKey(currentTag))
					_data[currentTag] = new List<string>();

				_data[currentTag].Add(message);
			}

			if (currentTag.Date == DateTime.MinValue)
				return false; // no tags found

			#region Html
			if (_outputType == OutputType.Html)
			{
				outStream.WriteLine("<html>\n<body>");

				foreach (var tag in _data.OrderByDescending(x => x.Key.Date))
				{
					outStream.WriteLine($"<b style=\"background-color:darkgreen;color:white\">&nbsp;{tag.Key.Tag}&nbsp;</b>");

					if (tag.Key.Branch != "")
						outStream.WriteLine($"<b style=\"background-color:darkred;color:white\">{tag.Key.Branch}</b>");

					outStream.WriteLine($"<table>\n<tr><td><b>{tag.Key.Date.ToLongDateString()}</b></td></tr>");

					foreach (var message in tag.Value)
						outStream.WriteLine($"<tr><td>&nbsp;&nbsp;{message}</td></tr>");

					outStream.WriteLine("</table>\n<br>");
				}
				outStream.WriteLine("</body>\n</html>");
			}
			#endregion
			#region Markdown
			else if (_outputType == OutputType.Markdown)
			{
				foreach (var tag in _data.OrderByDescending(x => x.Key.Date))
				{
					outStream.WriteLine($"#### {tag.Key.Tag}\n> {tag.Key.Date.ToLongDateString()}");

					foreach (var message in tag.Value)
						outStream.WriteLine($"- {message}");
				}
			}
			#endregion
			#region Text
			else
			{
				foreach (var tag in _data.OrderByDescending(x => x.Key.Date))
				{
					outStream.WriteLine($"{tag.Key.Tag} {tag.Key.Date.ToLongDateString()}"); // Don't show branch

					foreach (var message in tag.Value)
						outStream.WriteLine($"  {message}");
				}
			}
			#endregion

			_data.Clear();

			return true;
		}
	}
}
