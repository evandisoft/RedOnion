using RedOnion.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace RedOnion.Debugging
{
	public static class MainLogger
	{
		/// <summary>
		/// Culture settings for formatting (invariant by default).
		/// </summary>
		public static CultureInfo Culture = CultureInfo.InvariantCulture;

		public static string Format(FormattableString msg)
			=> msg.ToString(Culture);
		public static string Format(StringWrapper msg, params object[] args)
			=> args?.Length > 0 ? string.Format(Culture, msg.String, args) : msg.String;

		public static Action<string> LogListener;

		public static void Log(FormattableString msg)
			=> LogListener?.Invoke(msg.ToString(Culture));
		public static void Log(StringWrapper msg, params object[] args)
			=> LogListener?.Invoke(Format(msg, args));
		[Conditional("DEBUG")]
		public static void DebugLog(FormattableString msg)
			=> LogListener?.Invoke(msg.ToString(Culture));
		[Conditional("DEBUG")]
		public static void DebugLog(StringWrapper msg, params object[] args)
			=> LogListener?.Invoke(Format(msg, args));

#if DEBUG
		public static Action<string> ExtraLogListener;
		[Conditional("DEBUG")]
		public static void ExtraLog(FormattableString msg)
			=> ExtraLogListener?.Invoke(msg.ToString(Culture));
		[Conditional("DEBUG")]
		public static void ExtraLog(StringWrapper msg, params object[] args)
			=> ExtraLogListener?.Invoke(Format(msg, args));
#else
		[Conditional("DEBUG")]
		public static void ExtraLog(FormattableString msg) { }
		[Conditional("DEBUG")]
		public static void ExtraLog(StringWrapper msg, params object[] args) { }
#endif
	}
}
