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
			public int NumTabs;
			public List<ObjectLine> Children = new List<ObjectLine>();
		}

		const string EndProcessingString = "LuaFunction::PerformCall - 'gd.test_output.dumpit'";
		static string[] LogLines;
		static List<ObjectLine> ObjectLines;
		static int TrimLength = 0;

		static void GetTextOfLine(string text, ObjectLine ol)
		{
			if (TrimLength == 0) TrimLength = text.IndexOf(':') + 1;
			ol.Text = text.Substring(TrimLength).Replace("</Release>", "");
			ol.NumTabs = ol.Text.Where(c => c == '\t').Count();
			ol.Text = ol.Text.Trim();
		}

		/*static int ProcessForObjectLine(ObjectLine pol, int line)
		{
			for (; line < LogLines.Length; line++)
			{
				if (LogLines[line].Contains(EndProcessingString)) return line;
				ObjectLine col = new ObjectLine();
				GetTextOfLine(LogLines[line], col);
				if (col.NumTabs > pol.NumTabs)
				{
					pol.Children.Add(col);
					line = ProcessForObjectLine(col, line + 1);
				}
			}
		}*/

		static void PrintObjectLine(ObjectLine ol)
		{
			string output = "";
			for (int i = 0; i < ol.NumTabs; i++) output += "->";
			Console.WriteLine(output + ol.Text);
			for (int i = 0; i < ol.Children.Count; i++)
				PrintObjectLine(ol.Children[i]);
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
			Stack<ObjectLine> OLStack = new Stack<ObjectLine>();
			ObjectLine CurOL = null;
			bool processing = false;
			for (int i = 0; i < LogLines.Length; i++)
			{
				if (!processing)
				{
					if (LogLines[i].Contains("LuaGlue::GetFunction - gd.test_output.dumpit: succeeded")) processing = true;
				}
				else if (LogLines[i].Contains(EndProcessingString))
				{
					break;
				}
				else
				{
					ObjectLine ol = new ObjectLine();
					GetTextOfLine(LogLines[i], ol);
					if (CurOL == null)
					{
						CurOL = ol;
						ObjectLines.Add(CurOL);
					}
					else
					{
						// ol is a child of CurOL
						if (CurOL.NumTabs < ol.NumTabs)
						{
							CurOL.Children.Add(ol);
							OLStack.Push(CurOL);
							CurOL = ol;
						}
						// ol is either a sibling or parent of CurOL
						else
						{
							int nt = CurOL.NumTabs;
							while (OLStack.Count > 0)
							{
								CurOL = OLStack.Pop();
								if (CurOL.NumTabs < nt)
								{
									CurOL.Children.Add(ol);
									nt = -1;
									break;
								}
							}

							if (OLStack.Count == 0 && nt > -1)
							{
								ObjectLines.Add(ol);
								CurOL = ol;
							}
						}
					}
				}
			}

			// output just to ensure we're doing this right
			for (int i = 0; i < ObjectLines.Count; i++)
				PrintObjectLine(ObjectLines[i]);

			Console.Read();
		}
	}
}
