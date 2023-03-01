using System;
using System.IO;
using System.Text;

using ChangeLogFormatter;
using NUnit.Framework;

namespace ChangeLogFormatterTest
{
	public class ChangeLogFormatterTests
	{
		Parser _parser = new Parser();
		StreamWriter _output;

		[SetUp]
		public void Setup()
		{
			_output = new StreamWriter(new MemoryStream());
		}

		[Test]
		public void NoTag()
		{
			_parser.Parse(new[] { "24/02/23 Message" }, _output, Parser.OutputType.Text);

			var results = Content(_output);
			Assert.IsEmpty(results[0]);
		}

		[Test]
		public void BadParse()
		{
			_parser.Parse(new[] { "Foobar" }, _output, Parser.OutputType.Text);

			var results = Content(_output);
			Assert.IsEmpty(results[0]);
		}


		[Test]
		public void Tag()
		{
			_parser.Parse(new[] { "24/02/23 (tag: 1.0.1) Message" }, _output, Parser.OutputType.Text);

			var results = Content(_output);
			Assert.True(results[0].StartsWith("1.0.1"));
		}

		[Test]
		public void Tags_HeadMaster()
		{
			_parser.Parse(new[] { "24/02/23 (HEAD -> master, tag: 1.0.2) Message" }, _output, Parser.OutputType.Text);

			var results = Content(_output);
			Assert.True(results[0].StartsWith("1.0.2"));
		}

		[Test]
		public void Tag_OriginMaster()
		{
			_parser.Parse(new[] { "24/02/23 (tag: 1.0.3, origin/master) Message" }, _output, Parser.OutputType.Text);

			var results = Content(_output);
			Assert.True(results[0].StartsWith("1.0.3"));
		}


		[Test]
		public void Tag_headMaster_OriginMaster()
		{
			_parser.Parse(new[] { "24/02/23 (HEAD -> master, tag: 1.0.4, origin/master) Message" }, _output, Parser.OutputType.Text);

			var results = Content(_output);
			Assert.True(results[0].StartsWith("1.0.4"));
		}

		[Test]
		public void Tag_headMaster_OriginMaster_OriginHead()
		{
			_parser.Parse(new[] { "24/02/23 (HEAD -> master, tag: 1.0.5, origin/master, origin/HEAD) Message" }, _output, Parser.OutputType.Text);

			var results = Content(_output);
			Assert.True(results[0].StartsWith("1.0.5"));
		}


		private string[] Content(StreamWriter stream)
		{
			stream.Flush();
			return Encoding.ASCII.GetString(((MemoryStream)stream.BaseStream).ToArray()).Split('\n');
		}
	}
}
