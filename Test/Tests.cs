using System;
using System.IO;

using ChangeLogFormatter;
using NUnit.Framework;

namespace ChangeLogFormatterTest
{
	public class ChangeLogFormatterTests
	{
		GenerateReports _parser;
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
			_parser = new GenerateReports(GenerateReports.OutputType.Text, outFile);
			Assert.IsTrue(_parser.Generate(_repoPath));
			outFile.Close();

			outFile = new StreamWriter("changelog.md");
			_parser = new GenerateReports(GenerateReports.OutputType.Markdown, outFile);
			Assert.IsTrue(_parser.Generate(_repoPath));
			outFile.Close();

			outFile = new StreamWriter("changelog.rtf");
			_parser = new GenerateReports(GenerateReports.OutputType.Rtf, outFile);
			Assert.IsTrue(_parser.Generate(_repoPath));
			outFile.Close();

			outFile = new StreamWriter("changelog.htm");
			_parser = new GenerateReports(GenerateReports.OutputType.Html, outFile);
			Assert.IsTrue(_parser.Generate(_repoPath));
			outFile.Close();
		}
	}
}
