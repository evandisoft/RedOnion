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
			Complogger = new QueueLogger("completion", 1000);
			UILogger = new QueueLogger("ui", 1000);
			UndoLogger = new QueueLogger("undoredo", 1000);
			MunLogger = new QueueLogger("munos", 1000);
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
			FieldInfo[] fields = typeof(QueueLogger).GetFields(BindingFlags.Static | BindingFlags.Public);
			StringBuilder sb=new StringBuilder();
			foreach (var field in fields)
			{
				if (field.GetValue(null) is QueueLogger logger)
				{
					if (logger.HasTag(tag))
					{
						foreach (var str in logger.logQueue)
						{
							sb.AppendLine(str);
						}
					}
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

		Queue<string> logQueue = new Queue<string>();
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
			logQueue.Enqueue(msg);
		}

		public void LogArray(params object[] args)
			=> Push(ObjectArrayToString(args));
		[Conditional("DEBUG")]
		public void DebugLogArray(params object[] args)
			=> Push(ObjectArrayToString(args));

		string ObjectArrayToString(object[] args)
		{
			StringBuilder sb = new StringBuilder();
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

		public string GetContents()
		{
			StringBuilder sb = new StringBuilder();
			foreach (var str in logQueue)
			{
				sb.AppendLine(str);
			}
			return sb.ToString();
		}
	}
}
