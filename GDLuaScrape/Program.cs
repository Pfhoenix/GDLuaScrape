using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GDLuaScrape
{
	class Program
	{
		class ObjectLine
		{
			public string Text;
			public List<ObjectLine> Children = new List<ObjectLine>();
		}

		static string[] LogLines;
		static List<ObjectLine> ObjectLines;
		static int trimlength = 0;

		static int ProcessForObjectLine(ObjectLine ol, int line)
		{
			// first thing is to get the name for the ObjectLine
			if (trimlength == 0) trimlength = LogLines[line].IndexOf(':') + 1;
			ol.Text = LogLines[line].Substring(trimlength).Replace("</Release>", "");
			int mc = ol.Text.Where(c => c == '\t').Count();
			ol.Text = ol.Text.Trim();
			for (; line < LogLines.Length; line++)
			{

			}
		}

		static void Main(string[] args)
		{
			string logfilepath = @"C:\Users\Pfhoenix\Documents\My Games\Grim Dawn\log.xml";
			if (!File.Exists(logfilepath))
			{
				Console.Read();
				return;
			}
			LogLines = File.ReadAllLines(logfilepath);
			ObjectLines = new List<ObjectLine>();
			bool processing = false;
			for (int i = 0; i < LogLines.Length; i++)
			{
				if (!processing)
				{
					if (LogLines[i].Contains("LuaGlue::GetFunction - gd.test_output.dumpit: succeeded")) processing = true;
				}
				else if (LogLines[i].Contains("LuaFunction::PerformCall - 'gd.test_output.dumpit'"))
				{
					break;
				}
				else
				{
					ObjectLine ol = new ObjectLine();
					ObjectLines.Add(ol);
					// we expect the return to be the index of the line that is outside this object, so we want to start processing it next loop
					i = ProcessForObjectLine(ol, i) - 1;
				}
			}
		}
	}
}
