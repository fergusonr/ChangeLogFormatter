using System;
using System.IO;

using ChangeLogFormatter;
using NUnit.Framework;

namespace ChangeLogFormatterTest
{
	public class ChangeLogFormatterTests
	{
		Parser _parser = new Parser(Parser.OutputType.Text);
		StreamWriter _output;

		[SetUp]
		public void Setup()
		{
			_output = new StringStream();
		}

		[Test]
		public void NoTag()
		{
			_parser.Parse(new[] { "24/02/23 Message" }, _output);
			Assert.IsEmpty(_output.ToString());
		}

		[Test]
		public void BadParse()
		{
			_parser.Parse(new[] { "Foobar" }, _output);
			Assert.IsEmpty(_output.ToString());
		}


		[Test]
		public void Tag()
		{
			_parser.Parse(new[] { "24/02/23 (tag: 1.0.1) Message" }, _output);
			Assert.AreEqual("1.0.1 24 February 2023\r\n  Message\r\n", _output.ToString());
		}

		[Test]
		public void Tags_HeadMaster()
		{
			_parser.Parse(new[] { "24/02/23 (HEAD -> master, tag: 1.0.2) Message" }, _output);
			Assert.AreEqual("1.0.2 24 February 2023\r\n  Message\r\n", _output.ToString());
		}

		[Test]
		public void Tag_OriginMaster()
		{
			_parser.Parse(new[] { "24/02/23 (tag: 1.0.3, origin/master) Message" }, _output);
			Assert.AreEqual("1.0.3, origin/master 24 February 2023\r\n  Message\r\n", _output.ToString());
		}


		[Test]
		public void Tag_headMaster_OriginMaster()
		{
			_parser.Parse(new[] { "24/02/23 (HEAD -> master, tag: 1.0.4, origin/master) Message" }, _output);
			Assert.AreEqual("1.0.4, origin/master 24 February 2023\r\n  Message\r\n", _output.ToString());
		}

		[Test]
		public void Tag_headMaster_OriginMaster_OriginHead()
		{
			_parser.Parse(new[] { "24/02/23 (HEAD -> master, tag: 1.0.5, origin/master, origin/HEAD) Message" }, _output);
			Assert.AreEqual("1.0.5, origin/master, origin/HEAD 24 February 2023\r\n  Message\r\n", _output.ToString());
		}
	}
}
