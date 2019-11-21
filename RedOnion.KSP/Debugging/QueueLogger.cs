using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace RedOnion.KSP.Debugging {
    public class QueueLogger {
        static QueueLogger()
        {
            RegisteredTags = new HashSet<string>();
            Compl = new QueueLogger("completion", 1000);
			UILogger = new QueueLogger("ui", 1000);
		}

        static public QueueLogger Compl;
		static public QueueLogger UILogger;
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
							sb.Append(str);
						}
					}
				}
			}
			return sb.ToString();
		}

		static public HashSet<string> Tags(params string[] strings)
        {
            var tags = new HashSet<string>();
            foreach (var str in strings) {
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

        public QueueLogger(string filename,int queueSize = defaultQueueSize)
        {
            Init(filename, Tags(), queueSize);
        }
        public QueueLogger(string filename,HashSet<string> tags,int queueSize=defaultQueueSize)
        {
            Init(filename, tags, queueSize);
        }

        void Init(string filename, HashSet<string> otherTags, int newQueueSize)
        {
            logpath = basePath + filename + ".log";

            string filenameTag = filename.ToLower();
            tags.Add(filenameTag);
            RegisteredTags.Add(filenameTag);

            foreach (var tag in otherTags) {
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

        public void Log(params object[] args)
        {
            if (logQueue.Count >= queueSize) {
                logQueue.Dequeue();
            }
            logQueue.Enqueue(ObjectArrayToString(args));
        }

        string ObjectArrayToString(object[] args)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var arg in args) {
                if (arg == null) {
                    sb.Append("null");
                } else {
                    sb.Append(arg.ToString());
                }
                sb.Append(",");
            }
            sb.Append("\n");
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
				sb.Append(str);
			}
			return sb.ToString();
		}
	}
}
