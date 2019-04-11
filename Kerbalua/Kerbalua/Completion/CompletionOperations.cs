using System.Text;
using System.Collections.Generic;

namespace Kerbalua.Completion
{
	public class CompletionOperations
	{
		public List<CompletionOperation> OperationList = new List<CompletionOperation>();
		public int Index {get; private set;}
		public bool IsFinished
		{
			get
			{
				return Index >= OperationList.Count;
			}
		}
		public bool LastOperation => Index == OperationList.Count - 1;

		public override string ToString()
		{
			var sb = new StringBuilder();

			sb.Append("Index: " + Index);
			sb.Append(", IsFinished: " + IsFinished+", ");
			sb.Append("Current: " + Current + ", ");
			sb.Append("All: ");
			foreach (var operation in OperationList)
			{
				sb.Append(operation + ", ");
			}
			sb.Remove(sb.Length - 2, 2);
			return sb.ToString();
		}

		public CompletionOperation Current
		{
			get
			{
				if(Index >= OperationList.Count || Index < 0)
				{
					return null;
				}

				return OperationList[Index];
			}
		}
		public CompletionOperation Peek(int i)
		{
			if (Index + i >= OperationList.Count || Index + i < 0)
			{
				return null;
			}

			return OperationList[Index + i];
		}

		public void Reset()
		{
			Index = 0;
		}

		public bool MoveNext()
		{
			return Move(1);
		}

		public bool Move(int i)
		{
			Index += i;
			return IsFinished;
		}

		public CompletionOperations(IList<Segment> segments)
		{
			if (segments.Count == 0)
			{
				OperationList.Add(new GetMemberOperation(""));
			}
			foreach (var segment in segments)
			{
				OperationList.Add(new GetMemberOperation(segment.Name));
				foreach(var part in segment.Parts)
				{
					if(part is CallPart callPart)
					{
						OperationList.Add(new CallOperation());
					}
					else if(part is ArrayPart arrayPart)
					{
						OperationList.Add(new ArrayAccessOperation());
					}
					else
					{
						throw new LuaIntellisenseException("Expected Call or ArrayAccess operation for segment " + segment.Name);
					}

				}
			}

			Reset();
		}
	}
}
