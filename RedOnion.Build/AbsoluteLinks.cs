using System;
using System.IO;
using System.Text.RegularExpressions;

namespace RedOnion.Build
{
	/// <summary>
	/// Takes the Readme and produces a version with absolute links for spacedock.
	/// 
	/// Changelog is not meant to be copied in whole to spacedock, just the individual part of the changelog that
	/// is associated with the new version.
	/// So links in older versions being outdated is irrelevant because they should not be copied in anymore.
	/// </summary>
	public static class AbsoluteLinks
	{
		public const string ReadmeFilename="README.md";
		public const string ChangelogFilename="ChangeLog.md";
		public const string ReadmeForSpacedockFilename="ReadmeForSpacedock.md";
		public const string ChangelogForSpacedockFilename="ChangelogForSpacedock.md";
		public const string BaseURL="https://github.com/evandisoft/RedOnion/";

		public static string GetAbsoluteLinksString(string readmeText,string absoluteBaseURL)
		{
			return Regex.Replace(readmeText, "\\]\\(", (match) =>
			{
				try
				{
					if (readmeText.Substring(match.Index+2, 8)=="https://")
					{
						return match.ToString();
					}

					return match+BaseURL;
				}
				catch(Exception)
				{
					return match.ToString();
				}

			});
		}

		public static void WriteAbsoluteLinksVersion(string inputFilename,string outputFilename,string baseURL)
		{
			if (!File.Exists(inputFilename))
			{
				throw new Exception("Could not find "+inputFilename+" in dir "+Directory.GetCurrentDirectory());
			}

			string readmeText=File.ReadAllText(inputFilename);

			string finalText=GetAbsoluteLinksString(readmeText,baseURL);

			File.WriteAllText(outputFilename, finalText);
		}

		public static void Exec()
		{
			WriteAbsoluteLinksVersion(ReadmeFilename, ReadmeForSpacedockFilename, BaseURL);
			WriteAbsoluteLinksVersion(ChangelogFilename, ChangelogForSpacedockFilename, BaseURL);
		}
	}
}
