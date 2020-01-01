using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunOS
{
	/// <summary>
	/// Unique identifier of a thread or process.
	/// </summary>
	public readonly struct MunID : IEquatable<MunID>, IComparable<MunID>, IFormattable
	{
		public readonly long id;
		private MunID(long id) => this.id = id;
		public static implicit operator long(MunID id) => id.id;

		public static readonly MunID Zero = new MunID(0);

		static long _lastID;    // last used positive ID (zero on init - never used)
		static long _lastNeg;   // last used negative ID (zero on init - never used)

		/// <summary>
		/// Positive IDs are to be used by processes and long-lived threads
		/// (MunPriority: Realtime, Idle and Main).
		/// </summary>
		/// <returns>New unique positive ID.</returns>
		public static MunID GetPositive() => new MunID(++_lastID);
		/// <summary>
		/// Negative IDs are to be used by short-lived threads
		/// (MunPriority: Callback).
		/// </summary>
		/// <returns>New unique negative ID.</returns>
		public static MunID GetNegative() => new MunID(--_lastNeg);

		public bool Equals(MunID other) => id == other.id;
		public override bool Equals(object obj) => obj is MunID other && Equals(other);
		public override int GetHashCode() => id.GetHashCode();
		public int CompareTo(MunID other) => id.CompareTo(other.id);

		public override string ToString() => id.ToString();
		public string ToString(string format, IFormatProvider formatProvider) => id.ToString(format, formatProvider);
	}
}
