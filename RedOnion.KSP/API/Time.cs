using MoonSharp.Interpreter;
using RedOnion.Attributes;
using RedOnion.ROS;
using RedOnion.ROS.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using KSPTW = global::TimeWarp;

namespace RedOnion.KSP.API
{
	[Callable("now")]
	[Description("The simulation time, in seconds, since the game or this save was started."
		+ " Returns `now` when used as a function or `TimeDelta` when number is provided.")]
	public static class Time
	{
		[Description("The simulation time, since the game or this save was started. Suitable for measuring time and printing.")]
		public static TimeStamp now => new TimeStamp(Planetarium.GetUniversalTime());
		[Description("No time (contains `NaN`). Useful for initialization of time-stamp variables as `since(none) = inf`.")]
		public static TimeStamp none => TimeStamp.none;

		[Description("The simulation time in seconds, since the game or this save was started. For pure computation (same as `now.seconds`).")]
		public static double seconds => Planetarium.GetUniversalTime();
		[Description("Time delta/span since some previous time.  Returns `infinite` if `time` is `none`. (Use `.seconds` on the result or `secondsSince` function if you want pure `double` value).")]
		public static TimeDelta since(TimeStamp time)
			=> new TimeDelta(double.IsNaN(time.seconds)
			? double.PositiveInfinity
			: now.seconds - time.seconds);
		[Description("Version of `since(TimeStamp)` accepting double - for compatibility and variables initialized to `NaN` or `-inf`.")]
		public static TimeDelta since(double time)
			=> new TimeDelta(double.IsNaN(time)
			? double.PositiveInfinity
			: now.seconds - time);
		[Description("Seconds since some previous time. Returns `+Inf` if `time` is `NaN`.")]
		public static double secondsSince(TimeStamp time)
			=> double.IsNaN(time.seconds)
			? double.PositiveInfinity
			: now.seconds - time.seconds;

		[Description("Time delta/span of one tick. (Script engine always runs in physics ticks.)")]
		public static TimeDelta tick => new TimeDelta(KSPTW.fixedDeltaTime);

		public static readonly Type warp = typeof(TimeWarp);
		public static readonly Type stamp = typeof(TimeStamp);
		public static readonly Type delta = typeof(TimeDelta);
		public static readonly Type span = typeof(TimeDelta);
	}

	[Description("Time warping utilities")]
	public static class TimeWarp
	{
		[Description("Warp-to engaged / in progress. Looks for `TimeWarpTo` lock.")]
		public static bool engaged => KSPTW.fetch.setAutoWarp || InputLockManager.lockStack.ContainsKey("TimeWarpTo");

		[WorkInProgress, Description("Indicator that `warp.to` can be used.")]
		public static bool ready
		{
			get
			{
				if (engaged)
					return false;
				var vessel = HighLogic.LoadedSceneIsFlight ? FlightGlobals.ActiveVessel : null;
				if (!vessel)
					return true;
				if (vessel.LandedOrSplashed)
					return true;
				if (vessel.staticPressurekPa > 0.0)
					return false;
				if (vessel.geeForce > KSPTW.GThreshold)
					return false;
				if (KSPTW.fetch.GetMaxRateForAltitude(vessel.altitude, vessel.mainBody) == 0)
					return false;
				return true;
			}
		}

		[Description("Warp mode set to low aka physics warp. Note that it can only be changed on zero rate-index (and when not `engaged`).")]
		public static bool low
		{
			get => KSPTW.WarpMode == KSPTW.Modes.LOW;
			set => setMode(KSPTW.Modes.LOW);
		}
		[Description("Warp mode set to high aka on-rails warp. Note that it can only be changed on zero rate-index (and when not `engaged`).")]
		public static bool high
		{
			get => KSPTW.WarpMode == KSPTW.Modes.HIGH;
			set => setMode(KSPTW.Modes.HIGH);
		}
		private static void setMode(KSPTW.Modes mode)
		{
			if (engaged || KSPTW.fetch.Mode == mode || KSPTW.CurrentRateIndex > 0)
				return;
			KSPTW.fetch.Mode = mode;
		}

		[Description("Current rate.")]
		public static float rate
		{
			get => KSPTW.CurrentRate;
			set => setRate(value);
		}
		[Description("Current rate index.")]
		public static int index
		{
			get => KSPTW.CurrentRateIndex;
			set => setIndex(value);
		}

		private static float[] rates
			=> low ? KSPTW.fetch.physicsWarpRates : KSPTW.fetch.warpRates;
		[Description("Set rate index. Returns false if not possible now.")]
		public static bool setIndex(int value)
		{
			if (engaged)
				return false;
			var rates = TimeWarp.rates;
			value = RosMath.Clamp(value, 0, rates.Length-1);
			KSPTW.SetRate(value, false);
			return true;
		}
		[Description("Set rate. Returns false if not possible now.")]
		public static bool setRate(float value)
		{
			if (!ready)
				return false;
			var rates = TimeWarp.rates;
			float closest = float.MaxValue;
			int i = 0;
			while (i < rates.Length)
			{
				float diff = Math.Abs(value - rates[i]);
				if (diff > closest)
					break;
				closest = diff;
				i++;
			}
			return setIndex(i-1);
		}

		[Description("Warp to specified time. Returns true if engaged, false if not ready.")]
		public static bool to(TimeStamp time)
			=> to(time.seconds);
		public static bool to(double time)
		{
			if (!ready)
				return false;
			KSPTW.fetch.WarpTo(time);
			return KSPTW.fetch.setAutoWarp;
		}

		[Description("Cancel any warp-to in progress. Returns true if it was canceled, false if no warp was in progress.")]
		public static bool cancel()
		{
			if (KSPTW.fetch == null)
				return false;
			KSPTW.fetch.setAutoWarp = false;
			return KSPTW.fetch.CancelAutoWarp();
		}
	}

	[Description("Absolute time (and date). (Like `DateTime`)")]
	public struct TimeStamp : IEquatable<TimeStamp>, IComparable<TimeStamp>, IConvert, IOperators
	{
		[Description("The contained value - seconds since start of the game (or save).")]
		public double seconds;
		public TimeStamp(double seconds) => this.seconds = seconds;
		public static implicit operator double(TimeStamp stamp) => stamp.seconds;
		public static explicit operator TimeStamp(double seconds) => new TimeStamp(seconds);

		[Description("The time-stamp separated into parts (second, hour, ...).")]
		public TimeParts parts => new TimeParts(seconds);

		[Description("No time (contains `NaN`). Useful for initialization of time-stamp variables as `time.since(none) = inf`.")]
		public static readonly TimeStamp none = new TimeStamp(double.NaN);

		public static TimeStamp operator +(TimeStamp stamp, TimeDelta delta) => new TimeStamp(stamp.seconds + delta.seconds);
		public static TimeStamp operator -(TimeStamp stamp, TimeDelta delta) => new TimeStamp(stamp.seconds - delta.seconds);

		public static TimeStamp operator +(TimeStamp stamp, double seconds) => new TimeStamp(stamp.seconds + seconds);
		public static TimeStamp operator -(TimeStamp stamp, double seconds) => new TimeStamp(stamp.seconds - seconds);

		public static TimeDelta operator -(TimeStamp lhs, TimeStamp rhs) => new TimeDelta(lhs.seconds - rhs.seconds);

		public static bool operator ==(TimeStamp lhs, TimeStamp rhs) => lhs.seconds == rhs.seconds;
		public static bool operator !=(TimeStamp lhs, TimeStamp rhs) => lhs.seconds != rhs.seconds;
		public static bool operator >=(TimeStamp lhs, TimeStamp rhs) => lhs.seconds >= rhs.seconds;
		public static bool operator <=(TimeStamp lhs, TimeStamp rhs) => lhs.seconds <= rhs.seconds;
		public static bool operator >(TimeStamp lhs, TimeStamp rhs) => lhs.seconds > rhs.seconds;
		public static bool operator <(TimeStamp lhs, TimeStamp rhs) => lhs.seconds < rhs.seconds;

		bool IConvert.Convert(ref Value self, Descriptor to)
		{
			if (to.IsNumber)
			{
				self = seconds;
				if (to.Primitive == ExCode.Double)
					return true;
				return self.desc.Convert(ref self, to);
			}
			return false;
		}

		bool IOperators.Unary(ref Value self, OpCode op)
			=> false;
		bool IOperators.Binary(ref Value lhs, OpCode op, ref Value rhs)
		{
			if (lhs.desc.Type == typeof(TimeStamp))
			{
				var stamp = (TimeStamp)lhs.obj;
				if (rhs.desc.IsNumber)
				{
					var number = rhs.ToDouble();
					switch (op)
					{
					case OpCode.Add:
						lhs = new Value(lhs.desc, stamp + number);
						return true;
					case OpCode.Sub:
						lhs = new Value(lhs.desc, stamp - number);
						return true;
					}
					return false;
				}
				if (rhs.desc.Type == typeof(TimeDelta))
				{
					var delta = (TimeDelta)rhs.obj;
					switch (op)
					{
					case OpCode.Add:
						lhs = new Value(lhs.desc, stamp + delta);
						return true;
					case OpCode.Sub:
						lhs = new Value(lhs.desc, stamp - delta);
						return true;
					}
					return false;
				}
				if (rhs.desc.Type == typeof(TimeStamp))
				{
					var stamp2 = (TimeStamp)rhs.obj;
					switch (op)
					{
					case OpCode.Sub:
						lhs = new Value(lhs.desc, new TimeDelta(stamp.seconds - stamp2.seconds));
						return true;
					case OpCode.Equals:
						lhs = stamp == stamp2;
						return true;
					case OpCode.Differ:
						lhs = stamp != stamp2;
						return true;
					case OpCode.Less:
						lhs = stamp < stamp2;
						return true;
					case OpCode.More:
						lhs = stamp > stamp2;
						return true;
					case OpCode.LessEq:
						lhs = stamp <= stamp2;
						return true;
					case OpCode.MoreEq:
						lhs = stamp >= stamp2;
						return true;
					}
				}
				return false;
			}
			return false;
		}

		public override bool Equals(object obj)
			=> obj is TimeStamp stamp && seconds == stamp.seconds
			|| obj is double number && seconds == number;
		public override int GetHashCode()
			=> seconds.GetHashCode();
		public bool Equals(TimeStamp other)
			=> seconds == other.seconds;
		public int CompareTo(TimeStamp other)
			=> seconds.CompareTo(other.seconds);

		public override string ToString()
		{
			var p = parts;
			int year = p.negative ? -p.year-1 : p.year+1;
			return Value.Format($"{year:D2}/{p.day+1:D3} {p.hour:D2}:{p.minute:D2}:{p.second:D2}");
		}
	}
	[Description("Relative time. (Like `TimeSpan`)")]
	public struct TimeDelta : IEquatable<TimeDelta>, IComparable<TimeDelta>, IFormattable, IConvert, IOperators
	{
		[Description("The contained value - total seconds.")]
		public double seconds;
		[Description("Create new time-delta/span given seconds.")]
		public TimeDelta(double seconds) => this.seconds = seconds;
		public static implicit operator double(TimeDelta span) => span.seconds;
		public static explicit operator TimeDelta(double seconds) => new TimeDelta(seconds);

		public TimeDelta(TimeSpan span) => seconds = span.TotalSeconds;

		[Description("The time-delta/span separated into parts (second, hour, ...).")]
		public TimeParts parts => new TimeParts(seconds);

		[Description("No time-delta/span (contains `NaN`).")]
		public static readonly TimeDelta none = new TimeDelta(double.NaN);
		[Description("Infinite time-delta/span (contains `+Inf`).")]
		public static readonly TimeDelta infinite = new TimeDelta(double.PositiveInfinity);

		[Description("Total minutes (`seconds / 60`).")]
		public double minutes
		{
			get => seconds * (1.0/60.0);
			set => seconds = value * 60.0;
		}
		[Description("Total hours (`seconds / 3600`).")]
		public double hours
		{
			get => seconds * (1.0/3600.0);
			set => seconds = value * 3600.0;
		}

		public static TimeDelta operator +(TimeDelta delta) => delta;
		public static TimeDelta operator -(TimeDelta delta) => new TimeDelta(-delta.seconds);

		public static TimeDelta operator +(TimeDelta lhs, TimeDelta rhs) => new TimeDelta(lhs.seconds + rhs.seconds);
		public static TimeDelta operator -(TimeDelta lhs, TimeDelta rhs) => new TimeDelta(lhs.seconds - rhs.seconds);

		public static bool operator ==(TimeDelta lhs, TimeDelta rhs) => lhs.seconds == rhs.seconds;
		public static bool operator !=(TimeDelta lhs, TimeDelta rhs) => lhs.seconds != rhs.seconds;
		public static bool operator >=(TimeDelta lhs, TimeDelta rhs) => lhs.seconds >= rhs.seconds;
		public static bool operator <=(TimeDelta lhs, TimeDelta rhs) => lhs.seconds <= rhs.seconds;
		public static bool operator >(TimeDelta lhs, TimeDelta rhs) => lhs.seconds > rhs.seconds;
		public static bool operator <(TimeDelta lhs, TimeDelta rhs) => lhs.seconds < rhs.seconds;

		public static bool operator ==(TimeDelta delta, double seconds) => delta.seconds == seconds;
		public static bool operator !=(TimeDelta delta, double seconds) => delta.seconds != seconds;
		public static bool operator >=(TimeDelta delta, double seconds) => delta.seconds >= seconds;
		public static bool operator <=(TimeDelta delta, double seconds) => delta.seconds <= seconds;
		public static bool operator >(TimeDelta delta, double seconds) => delta.seconds > seconds;
		public static bool operator <(TimeDelta delta, double seconds) => delta.seconds < seconds;

		public static bool operator ==(double seconds, TimeDelta delta) => seconds == delta.seconds;
		public static bool operator !=(double seconds, TimeDelta delta) => seconds != delta.seconds;
		public static bool operator >=(double seconds, TimeDelta delta) => seconds >= delta.seconds;
		public static bool operator <=(double seconds, TimeDelta delta) => seconds <= delta.seconds;
		public static bool operator >(double seconds, TimeDelta delta) => seconds > delta.seconds;
		public static bool operator <(double seconds, TimeDelta delta) => seconds < delta.seconds;

		public static TimeDelta operator +(TimeDelta delta, double seconds) => new TimeDelta(delta.seconds + seconds);
		public static TimeDelta operator -(TimeDelta delta, double seconds) => new TimeDelta(delta.seconds - seconds);
		public static TimeDelta operator *(TimeDelta delta, double seconds) => new TimeDelta(delta.seconds * seconds);
		public static TimeDelta operator /(TimeDelta delta, double seconds) => new TimeDelta(delta.seconds / seconds);
		public static TimeDelta operator %(TimeDelta delta, double seconds) => new TimeDelta(delta.seconds % seconds);

		public static double operator +(double seconds, TimeDelta delta) => seconds + delta.seconds;
		public static double operator -(double seconds, TimeDelta delta) => seconds - delta.seconds;
		public static double operator *(double seconds, TimeDelta delta) => seconds * delta.seconds;
		public static double operator /(double seconds, TimeDelta delta) => seconds / delta.seconds;
		public static double operator %(double seconds, TimeDelta delta) => seconds % delta.seconds;

		public static TimeDelta operator ^(TimeDelta delta, double power) => new TimeDelta(Math.Pow(delta.seconds, power));
		public static double operator ^(double x, TimeDelta y) => Math.Pow(x, y.seconds);

		bool IConvert.Convert(ref Value self, Descriptor to)
		{
			if (to.IsNumber)
			{
				self = seconds;
				if (to.Primitive == ExCode.Double)
					return true;
				return self.desc.Convert(ref self, to);
			}
			return false;
		}

		bool IOperators.Unary(ref Value self, OpCode op)
		{
			switch (op)
			{
			case OpCode.Plus:
				return true;
			case OpCode.Neg:
				self.obj = new TimeDelta(-((TimeDelta)self.obj).seconds);
				return true;
			}
			return false;
		}
		bool IOperators.Binary(ref Value lhs, OpCode op, ref Value rhs)
		{
			double a, b;
			bool second = false;
			if (lhs.desc.Type == typeof(TimeDelta))
				a = ((TimeDelta)lhs.obj).seconds;
			else if (lhs.desc.IsNumber)
			{
				a = lhs.ToDouble();
				second = true;
			}
			else return false;
			if (rhs.desc.Type == typeof(TimeDelta))
				b = ((TimeDelta)rhs.obj).seconds;
			else if (rhs.desc.IsNumber)
				b = rhs.ToDouble();
			else return false;
			switch (op)
			{
			case OpCode.Add:
				lhs = second ? new Value(a + b) : new Value(lhs.desc, new TimeDelta(a + b));
				return true;
			case OpCode.Sub:
				lhs = second ? new Value(a - b) : new Value(lhs.desc, new TimeDelta(a - b));
				return true;
			case OpCode.Mul:
				lhs = second ? new Value(a * b) : new Value(lhs.desc, new TimeDelta(a * b));
				return true;
			case OpCode.Div:
				lhs = second ? new Value(a / b) : new Value(lhs.desc, new TimeDelta(a / b));
				return true;
			case OpCode.Mod:
				lhs = second ? new Value(a % b) : new Value(lhs.desc, new TimeDelta(a % b));
				return true;
			case OpCode.Power:
			case OpCode.BitXor:
				lhs = second ? new Value(Math.Pow(a, b)) : new Value(lhs.desc, new TimeDelta(Math.Pow(a, b)));
				return true;

			case OpCode.Equals:
				lhs = a == b;
				return true;
			case OpCode.Differ:
				lhs = a != b;
				return true;
			case OpCode.Less:
				lhs = a < b;
				return true;
			case OpCode.More:
				lhs = a > b;
				return true;
			case OpCode.LessEq:
				lhs = a <= b;
				return true;
			case OpCode.MoreEq:
				lhs = a >= b;
				return true;
			}
			return false;
		}

		public override bool Equals(object obj)
			=> obj is TimeDelta delta && seconds == delta.seconds
			|| obj is double d && seconds == d
			|| obj is float f && seconds == f
			|| obj is int i && seconds == i;
		public override int GetHashCode()
			=> seconds.GetHashCode();
		public bool Equals(TimeDelta other)
			=> seconds == other.seconds;
		public int CompareTo(TimeDelta other)
			=> seconds.CompareTo(other.seconds);

		public override string ToString()
		{
			var p = new TimeParts(Math.Round(seconds, Math.Abs(seconds) >= 3599.5 ? 0 : Math.Abs(seconds) >= 59.5 ? 1 : 2));
			if (p.year != 0)
			{
				int year = p.negative ? -p.year : p.year;
				return Value.Format($"{year:D}y{p.day:D}d{p.hour:D2}h{p.minute:D2}m{p.second:D2}s");
			}
			if (p.day != 0)
			{
				int day = p.negative ? -p.day : p.day;
				return Value.Format($"{day:D}d{p.hour:D2}h{p.minute:D2}m{p.second:D2}s");
			}
			if (p.hour != 0)
			{
				int hour = p.negative ? -p.hour : p.hour;
				return Value.Format($"{hour:D}h{p.minute:D2}m{p.second:D2}s");
			}
			if (p.minute != 0)
			{
				int minute = p.negative ? -p.minute : p.minute;
				return Value.Format($"{minute:D}m{p.second:D2}.{(int)(p.fraction*10):D1}s");
			}
			int second = p.negative ? -p.second : p.second;
			return Value.Format($"{second}.{(int)(p.fraction*100):D2}s");
		}

		public string ToString(string format, IFormatProvider provider)
		{
			// TODO: custom formatting
			if (format?.Length > 0)
			{
				switch (format[0])
				{
				case 'G':
				case 'F':
				case 'E':
				case 'N':
				case 'C':
					return seconds.ToString(format, provider);
				}
			}
			return ToString();
		}
	}

	[Description("TimeSpan or TimeDelta converted to its parts - hours, minutes, seconds etc.")]
	public struct TimeParts
	{
		[Description("Create TimeParts from seconds.")]
		public TimeParts(double seconds)
		{
			if (double.IsNaN(seconds))
			{
				fraction = double.NaN;
				negative = false;
				second = 0;
				minute = 0;
				hour = 0;
				day = 0;
				year = 0;
			}
			else if (double.IsInfinity(seconds))
			{
				fraction = double.PositiveInfinity;
				negative = seconds < 0;
				second = 0;
				minute = 0;
				hour = 0;
				day = 0;
				year = 0;
			}
			else
			{
				negative = seconds < 0;
				if (negative) seconds = -seconds;
				if (seconds <= int.MaxValue) // almost 233 kerbal years
				{
					int value = (int)Math.Floor(seconds);
					fraction = seconds - value;
					second = (byte)(value % 60);
					value /= 60;
					minute = (byte)(value % 60);
					value /= 60;
					hour = (byte)(value % 6);
					value /= 6;
					day = (ushort)(value % 426);
					year = (ushort)(value / 426);

				}
				else if (seconds <= 65536L*426*6*60*60-1) // insane limit
				{
					long value = (long)Math.Floor(seconds);
					fraction = (seconds - value);
					second = (byte)(value % 60);
					value /= 60;
					minute = (byte)(value % 60);
					value /= 60;
					hour = (byte)(value % 6);
					value /= 6;
					day = (ushort)(value % 426);
					year = (ushort)(value / 426);
				}
				else
				{
					fraction = float.PositiveInfinity;
					second = 0;
					minute = 0;
					hour = 0;
					day  = 0;
					year = 0;
				}
			}
		}

		[Description("Sub-seconds fraction. \\[0..1) or `Inf` or `NaN`")]
		public double fraction;
		[Description("Time is valid (`fraction` not `NaN`).")]
		public bool valid => !double.IsNaN(fraction);
		[Description("Time is finite (`fraction` not `NaN` or `Inf`).")]
		public bool finite => valid && !double.IsInfinity(fraction);
		[Description("Time is negative.")]
		public bool negative;
		[Description("Seconds part. \\[0..59]")]
		public byte second;
		[Description("Minutes part. \\[0..59]")]
		public byte minute;
		[Description("Hour part. \\[0..5]")]
		public byte hour;
		[Description("Day part. \\[0..425]")]
		public ushort day;
		[Description("Year part. \\[0..65535]")]
		public ushort year;
	}
}
