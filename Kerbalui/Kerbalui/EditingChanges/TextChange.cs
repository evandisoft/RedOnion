using System;
namespace Kerbalui.EditingChanges
{
	public struct TextChange
	{
		public int startIndex;
		public string originalString;
		public string replacementString;

		public static readonly TextChange NO_CHANGE;
		public static TextChange Calculate(string originalText, string newText)
		{
			int diffStart=0;
			for (; diffStart<originalText.Length; diffStart++)
			{
				if (diffStart>newText.Length-1)
				{
					// A string has been deleted from the end of the originalText
					return new TextChange(diffStart, originalText.Substring(diffStart), "");
				}
				// There is a change starting at diffStart
				if (originalText[diffStart]!=newText[diffStart])
				{
					break;
				}
			}

			if (diffStart==originalText.Length && diffStart==newText.Length)
			{
				// No changes haave occurred
				return NO_CHANGE;
			}

			if (diffStart==originalText.Length)
			{
				// A string has been added to the end of originalText
				return new TextChange(diffStart, "", newText.Substring(diffStart));
			}

			int lengthDiff=newText.Length-originalText.Length;

			int originalDiffEnd=originalText.Length-1;
			int newTextDiffEnd=originalDiffEnd+lengthDiff;

			if (lengthDiff>0)
			{
				// Replacement text is longer.
				for (; originalDiffEnd>diffStart; originalDiffEnd--, newTextDiffEnd--)
				{
					if (originalText[originalDiffEnd]!=newText[newTextDiffEnd])
					{
						return new TextChange(diffStart,
							originalText.Substring(diffStart, originalDiffEnd+1-diffStart),
							newText.Substring(diffStart, newTextDiffEnd+1-diffStart)
							);
					}
				}
			}
			else
			{
				// Original text is longer
				for (; newTextDiffEnd>diffStart; originalDiffEnd--, newTextDiffEnd--)
				{
					if (originalText[originalDiffEnd]!=newText[newTextDiffEnd])
					{
						return new TextChange(diffStart,
							originalText.Substring(diffStart, originalDiffEnd+1-diffStart),
							newText.Substring(diffStart, newTextDiffEnd+1-diffStart)
							);
					}
				}
			}

			return new TextChange(diffStart,
				originalText.Substring(diffStart, originalDiffEnd-diffStart),
				newText.Substring(diffStart, newTextDiffEnd-diffStart)
				);
		}

		public TextChange(int startIndex, string originalString,string replacementString)
		{
			this.startIndex=startIndex;
			this.originalString=originalString;
			this.replacementString=replacementString;
		}

		public override string ToString()
		{
			return $"[{startIndex},{originalString},{replacementString}]";
		}
	}
}
