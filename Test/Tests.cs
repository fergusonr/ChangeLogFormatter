using System;
using System.IO;

using ChangeLogFormatter;
using NUnit.Framework;

namespace ChangeLogFormatterTest
{
	public class ChangeLogFormatterTests
	{
		Parser _parser = new Parser(Parser.OutputType.Text);
		StringWriter _output;

		const string ExpectedResult = "1.0.1 24 February 2023\r\n  Message\r\n\r\n";

		[SetUp]
		public void Setup()
		{
			_output = new StringWriter();
		}

		[Test]
		public void Message()
		{
			Assert.IsTrue(_parser.Parse(new[] { "24/02/23 (tag: 1.0.1) Message1", "24/02/23  Message2" }, _output));
			Assert.AreEqual("1.0.1 24 February 2023\r\n  Message1\r\n  Message2\r\n\r\n", _output.ToString());
		}

		[Test]
		public void NoTag()
		{
			Assert.IsFalse(_parser.Parse(new[] { "24/02/23 Message" }, _output));
			Assert.IsEmpty(_output.ToString());
		}

		[Test]
		public void BadParse()
		{
			Assert.IsFalse(_parser.Parse(new[] { "Foobar" }, _output));
			Assert.IsEmpty(_output.ToString());
		}


		[Test]
		public void Tag()
		{
			Assert.IsTrue(_parser.Parse(new[] { "24/02/23 (tag: 1.0.1) Message" }, _output));
			Assert.AreEqual(ExpectedResult, _output.ToString());
		}

		[Test]
		public void Tags_HeadMaster()
		{
			Assert.IsTrue(_parser.Parse(new[] { "24/02/23 (HEAD -> master, tag: 1.0.1) Message" }, _output));
			Assert.AreEqual(ExpectedResult, _output.ToString());
		}

		[Test]
		public void Tag_OriginMaster()
		{
			Assert.IsTrue(_parser.Parse(new[] { "24/02/23 (tag: 1.0.1, origin/master) Message" }, _output));
			Assert.AreEqual("1.0.1, origin/master 24 February 2023\r\n  Message\r\n\r\n", _output.ToString());
		}


		[Test]
		public void Tag_headMaster_OriginMaster()
		{
			Assert.IsTrue(_parser.Parse(new[] { "24/02/23 (HEAD -> master, tag: 1.0.1, origin/master) Message" }, _output));
			Assert.AreEqual("1.0.1, origin/master 24 February 2023\r\n  Message\r\n\r\n", _output.ToString());
		}

		[Test]
		public void Tag_headMaster_OriginMaster_OriginHead()
		{
			Assert.IsTrue(_parser.Parse(new[] { "24/02/23 (HEAD -> master, tag: 1.0.1, origin/master, origin/HEAD) Message" }, _output));
			Assert.AreEqual("1.0.1, origin/master, origin/HEAD 24 February 2023\r\n  Message\r\n\r\n", _output.ToString());
		}
	}
}
