using System;
using System.IO;
using System.Text;

namespace ChangeLogFormatterTest
{
	/// <summary>
	/// StringBuilder inherited from StreamWriter
	/// </summary>
	public class StringStream : StreamWriter
	{
		public StringStream() : base(new MemoryStream())
		{
		}

		public override void Write(string message)
		{
			byte[] bytes = Encoding.ASCII.GetBytes(message);
			BaseStream.Write(bytes, 0, bytes.Length);
		}

		public override void WriteLine(string message)
		{
			Write(message);
			BaseStream.Write(new byte[] { 0x0d, 0x0a}, 0, 2);

		}
		public override string ToString()
		{
			BaseStream.Flush();
			return Encoding.ASCII.GetString(((MemoryStream)BaseStream).ToArray());
		}
	}
}
