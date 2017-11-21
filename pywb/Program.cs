using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace pywb
{
	class ExFileInfo
	{
		public FileInfo FInfo { get; set; }
		public string OutDir { get; set; }
	}

	class MainClass
	{
		public static void Main(string[] args)
		{
			if (args.Length < 1)
			{
				Console.WriteLine("Please specify one or more packages to transpile.");
				return;
			}
			foreach (var arg in args)
			{
				var files = GetPackage("src/"+arg+"/");
				Console.WriteLine("Transpiling " + files.Count + " files from package " + arg + "...");
				foreach (var file in files)
				{
					var o = File.ReadAllText(file.FInfo.FullName);
					o = GetPurePyStr(o);
					if (!Directory.Exists(file.OutDir.Substring(0, file.OutDir.Length - file.FInfo.Name.Length))) {
						Console.WriteLine("Creating needed directory " + file.OutDir.Substring(0, file.OutDir.Length - file.FInfo.Name.Length));
						Directory.CreateDirectory(file.OutDir.Substring(0, file.OutDir.Length - file.FInfo.Name.Length));
					}
					Console.WriteLine("Transpiling " + file.FInfo.Name + "...");
					File.WriteAllText(file.OutDir, o);
				}
			}
		}

		public static List<ExFileInfo> GetPackage(string pkg)
		{
			var o = new List<ExFileInfo>();
			try
			{
				foreach (string f in Directory.GetFiles(pkg))
				{
					if (f.EndsWith("pyw"))
						o.Add(new ExFileInfo { FInfo = new FileInfo(f), OutDir = f.Replace("src/", "out/").Replace(".pyw", ".py") });
				}
				foreach (string d in Directory.GetDirectories(pkg))
				{
					o.AddRange(GetPackage(d));
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
			return o;
		}

		public static string GetPurePyStr(string str)
		{
			string output = "";
			string current_token = "";
			string sub_token = "";
			string ch_token = "";
			bool is_b_str = false;
			bool in_string = false;
			bool in_cmt = false;
			string dind = "";
			char[] input = str.Replace("\t", "").ToCharArray();
			int ilevel = 0;
			for (int c = 0; c < input.Length; c++)
			{
				ch_token = input[c].ToString();

				if (!Environment.NewLine.Contains(ch_token))
				{
					if (ch_token == "\"" && !sub_token.EndsWith("\\"))
					{
						if (!in_cmt)
						{
							if (in_string)
							{
								in_string = false;
								if (!is_b_str)
									output += "u" + sub_token + "\"";
								else
									output += sub_token + "\"";
								is_b_str = false;
								sub_token = "";
								dind = "";
								c++;
							}
							else
							{
								if (current_token.EndsWith("s"))
								{
									is_b_str = true;
									current_token = current_token.Remove(current_token.Length - 1);
								}
								in_string = true;
								output += string.Format("{0}{1}", GetTabLines(ilevel), current_token);
								current_token = "";
							}
						}
					}

					ch_token = input[c].ToString();
					if (!in_string)
						current_token += input[c].ToString();
					else
						sub_token += input[c].ToString();


					if (!in_string)
					{
						if (!in_cmt)
						{
							current_token = current_token.Replace("this", "self").TrimStart();
							if (current_token.EndsWith("{"))
							{
								current_token = current_token.TrimStart();
								output += string.Format("{0}{1}:" + Environment.NewLine, dind, current_token.Remove(current_token.Length - 1));
								ilevel++;
								current_token = "";
								dind = GetTabLines(ilevel);
							}
							else if (current_token.EndsWith(";"))
							{
								current_token = current_token.TrimStart();
								output += string.Format("{0}{1}" + Environment.NewLine, dind, current_token);
								current_token = "";
								dind = GetTabLines(ilevel);
							}
							else if (current_token.EndsWith("}"))
							{
								output += string.Format("{0}{1}" + Environment.NewLine, dind, current_token.Remove(current_token.Length - 1));
								ilevel--;
								current_token = "";
								dind = GetTabLines(ilevel);
							}
							else if (current_token.EndsWith("func "))
							{
								current_token = current_token.TrimStart();
								output += string.Format("{1}{0}", current_token, dind).Replace("func", "def");
								current_token = "";
								dind = "";
							}
							else if (current_token.EndsWith("throw "))
							{
								current_token = current_token.TrimStart();
								output += string.Format("{1}{0}", current_token, dind).Replace("throw", "raise");
								current_token = "";
								dind = "";
							}
							else if (current_token.EndsWith("construct("))
							{
								output += string.Format("{1}{0}", current_token, dind).Replace("construct(", "__init__(");
								current_token = "";
								dind = "";
							}
							else if (current_token.EndsWith("iterate("))
							{
								output += string.Format("{1}{0}", current_token, dind).Replace("iterate(", "__iter__(");
								current_token = "";
								dind = "";
							}
							else if (current_token.EndsWith("new("))
							{
								output += string.Format("{1}{0}", current_token, dind).Replace("new(", "__new__(");
								current_token = "";
								dind = "";
							}
							/*else if (current_token.EndsWith("this"))
							{
								current_token = current_token.TrimStart();

								current_token = string.Format("{1}{0}", current_token, dind).Replace("this", "self");
							}*/
							else if (Regex.IsMatch(current_token, "(?:for\\s*\\()") && current_token.EndsWith(")"))
							{
								current_token = current_token.TrimStart();
								current_token = Regex.Replace(Regex.Replace(current_token, "(?:for\\s*\\()", "for "), "(\\))(?!.*\\))", "");
								output += string.Format("{1}{0}", current_token, dind).Replace("construct(", "__init__(");
								current_token = "";
								dind = "";
							}
							else if (current_token.StartsWith("/*") && current_token.EndsWith("*/"))
							{
								output += string.Format("{0}\"\"\"{1}\"\"\"" + Environment.NewLine, dind, current_token.Substring(2, current_token.Length - 4));
								current_token = "";
							}
							else if (current_token.StartsWith("//"))
							{
								in_cmt = true;
							}

						}
					}
				}
				else
				{
					if (in_cmt)
					{
						in_cmt = false;

						current_token = "";
					}
				}
			}
			return output.Replace("true", "True").Replace("false", "False");
		}

		private static string GetStringCont(string input1)
		{
			if (IsInString(input1))
			{
				for (int i = 0; i < input1.Length; i++)
				{

				}
			}
			return "";
		}

		internal static bool IsInString(string input1)
		{
			if (input1.IndexOf("\"") < 0)
				return false;
			else if (input1.IndexOf("\"") == input1.LastIndexOf("\""))
				return false;
			else
				return true;
		}

		internal static string GetTabLines(int count)
		{
			string str = "";
			for (int i = 0; i < count; i++)
				str += "\t";
			return str;
		}
	}
}
