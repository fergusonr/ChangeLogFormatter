using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using LibGit2Sharp;

namespace ChangeLogFormatter
{
	public class Parser
	{
		public enum OutputType { None, Text, Html, Markdown };

		private struct Tag
		{
			internal string Name;
			internal DateTime Date;
		}

		private readonly Dictionary<Tag, List<string>> _data = new Dictionary<Tag, List<string>>();
		private OutputType _outputType;
		private TextWriter _outStream;

		/// <summary>
		/// Git log parser
		/// </summary>
		/// <param name="type"></param>
		public Parser(OutputType type, TextWriter outStream)
		{
			_outputType = type;
			_outStream = outStream;
		}

		/// <summary>
		/// Format git log
		/// </summary>
		public bool Parse()
		{
			Repository repo;

			try
			{
				repo = new Repository(".");
			}
			catch(Exception e)
			{
				Console.Error.WriteLine(e.Message);
				return false;
			}

			var currentTag = new Tag();

			foreach (var commit in repo.Commits.OrderByDescending(x => x.Committer.When))
			{
				var tag = repo.Tags.FirstOrDefault(x => commit.Sha == x.Reference.TargetIdentifier);

				if (tag != null)
				{
					currentTag.Name = tag.FriendlyName;
					currentTag.Date = commit.Committer.When.Date;
				}

				if (currentTag.Date == DateTime.MinValue)
					continue;

				if (!_data.ContainsKey(currentTag))
					_data[currentTag] = new List<string>();

				_data[currentTag].Add(commit.MessageShort);
			}

			repo.Dispose();

			if (_data.Count == 0)
				return false; // no tags found

			#region Html
			if (_outputType == OutputType.Html)
			{
				_outStream.WriteLine("<html>\n<body>");

				foreach (var tag in _data.OrderByDescending(x => x.Key.Date))
				{
					_outStream.WriteLine($"<b style=\"background-color:darkgreen;color:white\">&nbsp;{tag.Key.Name}&nbsp;</b>");

					_outStream.WriteLine($"<table>\n<tr><td><b>{tag.Key.Date.ToLongDateString()}</b></td></tr>");

					foreach (var message in tag.Value)
						_outStream.WriteLine($"<tr><td>&nbsp;&nbsp;{message}</td></tr>");

					_outStream.WriteLine("</table>\n<br>");
				}
				_outStream.WriteLine("</body>\n</html>");
			}
			#endregion
			#region Markdown
			else if (_outputType == OutputType.Markdown)
			{
				foreach (var tag in _data.OrderByDescending(x => x.Key.Date))
				{
					_outStream.WriteLine($"#### {tag.Key.Name}\n> {tag.Key.Date.ToLongDateString()}");

					foreach (var message in tag.Value)
						_outStream.WriteLine($"- {message}");
				}
			}
			#endregion
			#region Text
			else
			{
				foreach (var tag in _data.OrderByDescending(x => x.Key.Date))
				{
					_outStream.WriteLine($"{tag.Key.Name} {tag.Key.Date.ToLongDateString()}"); // Don't show branch

					foreach (var message in tag.Value)
						_outStream.WriteLine($"  {message}");

					_outStream.WriteLine();
				}
			}
			#endregion

			_data.Clear();

			return true;
		}
	}
}
