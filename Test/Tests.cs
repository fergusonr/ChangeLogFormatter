using System;
using System.IO;

using ChangeLogFormatter;
using NUnit.Framework;

namespace ChangeLogFormatterTest
{
	public class ChangeLogFormatterTests
	{
		Parser _parser;
		string _repoPath;

		[OneTimeSetUp]
		public void Setup()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
			_repoPath = @"..\..\..";
		}

		[Test]
		public void GenerateAllLogTypes()
		{
			var outFile = new StreamWriter("changelog.txt");
			_parser = new Parser(Parser.OutputType.Text, outFile);
			Assert.IsTrue(_parser.Parse(_repoPath));
			outFile.Close();

			outFile = new StreamWriter("changelog.md");
			_parser = new Parser(Parser.OutputType.Markdown, outFile);
			Assert.IsTrue(_parser.Parse(_repoPath));
			outFile.Close();

			outFile = new StreamWriter("changelog.rtf");
			_parser = new Parser(Parser.OutputType.Rtf, outFile);
			Assert.IsTrue(_parser.Parse(_repoPath));
			outFile.Close();

			outFile = new StreamWriter("changelog.htm");
			_parser = new Parser(Parser.OutputType.Html, outFile);
			Assert.IsTrue(_parser.Parse(_repoPath));
			outFile.Close();
		}
	}
}
