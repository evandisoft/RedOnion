using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Globalization;
using RedOnion.Utilities;

namespace RedOnion.Debugging
{
	/// <summary>
	/// For now, this is disabled when project is not built in debug mode.
	/// </summary>
	public class QueueLogger
	{
		static QueueLogger()
		{
			RegisteredTags = new HashSet<string>();
			Complogger = new QueueLogger("completion");
			UILogger = new QueueLogger("ui");
			UndoLogger = new QueueLogger("undoredo");
			MunLogger = new QueueLogger("munos");
		}

		static public QueueLogger Complogger;
		static public QueueLogger UILogger;
		static public QueueLogger UndoLogger;
		static public QueueLogger MunLogger;

		static public HashSet<string> RegisteredTags;

		//static public void WriteTaggedToDisk(string tag = "all")
		//{
		//    FieldInfo[] fields = typeof(QueueLogger).GetFields(BindingFlags.Static | BindingFlags.Public);
		//    foreach (var field in fields) {
		//        if (field.GetValue(null) is QueueLogger logger) {
		//            if (logger.HasTag(tag)) {
		//                logger.WriteToDisk();
		//            }
		//        }
		//    }
		//}

		static public string GetContentsByTag(string tag = "all")
		{
			var sb = new StringBuilder();
			foreach (var field in typeof(QueueLogger).GetFields(BindingFlags.Static | BindingFlags.Public))
			{
				if (field.GetValue(null) is QueueLogger logger)
				{
					if (logger.HasTag(tag))
						logger.GetContents(sb);
				}
			}
			return sb.ToString();
		}

		static public void ClearLoggersByTag(string tag = "all")
		{
			FieldInfo[] fields = typeof(QueueLogger).GetFields(BindingFlags.Static | BindingFlags.Public);

			foreach (var field in fields)
			{
				if (field.GetValue(null) is QueueLogger logger)
				{
					logger.Clear();
				}
			}
		}

		static public HashSet<string> Tags(params string[] strings)
		{
			var tags = new HashSet<string>();
			foreach (var str in strings)
			{
				tags.Add(str);
			}
			return tags;
		}

		public void Clear()
		{
			logQueue.Clear();
		}

		const int defaultQueueSize = 1000;
		const string basePath = "Logs/Kerbalua/";

		public readonly struct Line
		{
			public readonly DateTime time;
			public readonly string message;
			public Line(string message)
			{
				time = DateTime.Now;
				this.message = message;
			}
			public override string ToString()
				=> string.Format(Culture, FormatString, time, message);
			public const string FormatString = "{0:HH:mm:ss.ff}: {1}";
		}
		Queue<Line> logQueue = new Queue<Line>();
		HashSet<string> tags = new HashSet<string>();

		string logpath;
		public int queueSize;

		public QueueLogger(string filename, int queueSize = defaultQueueSize)
		{
			Init(filename, Tags(), queueSize);
		}
		public QueueLogger(string filename, HashSet<string> tags, int queueSize = defaultQueueSize)
		{
			Init(filename, tags, queueSize);
		}

		void Init(string filename, HashSet<string> otherTags, int newQueueSize)
		{
			logpath = basePath + filename + ".log";

			string filenameTag = filename.ToLower();
			tags.Add(filenameTag);
			RegisteredTags.Add(filenameTag);

			foreach (var tag in otherTags)
			{
				tags.Add(tag.ToLower());
				RegisteredTags.Add(tag.ToLower());
			}

			string allTag = "all";
			tags.Add(allTag);
			RegisteredTags.Add(allTag);

			queueSize = newQueueSize;
		}

		public bool HasTag(string tag)
		{
			return tags.Contains(tag.ToLower());
		}

		/// <summary>
		/// Culture settings for formatting (invariant by default).
		/// </summary>
		public static CultureInfo Culture = CultureInfo.InvariantCulture;
		public void Log(FormattableString msg)
			=> Push(msg.ToString(Culture));
		public void Log(StringWrapper msg, params object[] args)
			=> Push(string.Format(Culture, msg.String, args));
		[Conditional("DEBUG")]
		public void DebugLog(FormattableString msg)
			=> Push(msg.ToString(Culture));
		[Conditional("DEBUG")]
		public void DebugLog(StringWrapper msg, params object[] args)
			=> Push(string.Format(Culture, msg.String, args));

		void Push(string msg)
		{
			if (logQueue.Count >= queueSize)
				logQueue.Dequeue();
			logQueue.Enqueue(new Line(msg));
		}

		public void LogArray(params object[] args)
			=> Push(ObjectArrayToString(args));
		[Conditional("DEBUG")]
		public void DebugLogArray(params object[] args)
			=> Push(ObjectArrayToString(args));

		string ObjectArrayToString(object[] args)
		{
			var sb = new StringBuilder();
			bool first = true;
			foreach (var arg in args)
			{
				if (!first)
					sb.Append(", ");
				if (arg == null)
				{
					sb.Append("null");
				}
				else
				{
					sb.Append(arg.ToString());
				}
				first = false;
			}
			return sb.ToString();
		}



		//public void WriteToDisk()
		//{
		//    StringBuilder sb = new StringBuilder();
		//    foreach(var str in logQueue) {
		//        sb.Append(str);
		//    }
		//    File.WriteAllText(logpath, sb.ToString());
		//}

		public void GetContents(StringBuilder sb)
		{
			foreach (var line in logQueue)
			{
				sb.AppendFormat(Line.FormatString, line.time, line.message);
				sb.AppendLine();
			}
		}
		public string GetContents()
		{
			var sb = new StringBuilder();
			GetContents(sb);
			return sb.ToString();
		}
	}
}
