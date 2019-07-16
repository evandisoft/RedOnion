using System;

namespace RedOnion.ROS.Utilities
{
	[System.ComponentModel.DisplayName("Math")]
	public static class RosMath
	{
		public const double PI = Math.PI;
		public const double E = Math.PI;
		public const double NaN = double.NaN;
		public const double Inf = double.PositiveInfinity;
		public const double Deg2Rad = PI / 180.0;
		public const double Rad2Deg = 180.0 / PI;

		public const float FPI = (float)PI;
		public const float FE = (float)E;
		public const float FNaN = (float)NaN;
		public const float FInf = (float)Inf;
		public const float FDeg2Rad = (float)Deg2Rad;
		public const float FRad2Deg = (float)Rad2Deg;

		public static class Deg
		{
			public static double Sin(double a) => Math.Sin(a * Deg2Rad);
			public static double Cos(double a) => Math.Cos(a * Deg2Rad);
			public static double Tan(double a) => Math.Tan(a * Deg2Rad);
			public static double Sinh(double a) => Math.Sinh(a * Deg2Rad);
			public static double Cosh(double a) => Math.Cosh(a * Deg2Rad);
			public static double Tanh(double a) => Math.Tanh(a * Deg2Rad);
			public static double Acos(double r) => Math.Acos(r) * Rad2Deg;
			public static double Asin(double r) => Math.Asin(r) * Rad2Deg;
			public static double Atan(double r) => Math.Atan(r) * Rad2Deg;
			public static double Atan2(double y, double x) => Math.Atan2(y, x) * Rad2Deg;

			public static float Sin(float a) => (float)Math.Sin(a * Deg2Rad);
			public static float Cos(float a) => (float)Math.Cos(a * Deg2Rad);
			public static float Tan(float a) => (float)Math.Tan(a * Deg2Rad);
			public static float Sinh(float a) => (float)Math.Sinh(a * Deg2Rad);
			public static float Cosh(float a) => (float)Math.Cosh(a * Deg2Rad);
			public static float Tanh(float a) => (float)Math.Tanh(a * Deg2Rad);
			public static float Acos(float r) => (float)(Math.Acos(r) * Rad2Deg);
			public static float Asin(float r) => (float)(Math.Asin(r) * Rad2Deg);
			public static float Atan(float r) => (float)(Math.Atan(r) * Rad2Deg);
			public static float Atan2(float y, float x) => (float)(Math.Atan2(y, x) * Rad2Deg);

			public static double Clamp360(double a)
			{
				a = a % 360;
				if (a < 0) a += 360;
				return a;
			}
		}

		public static double Abs(double value) => Math.Abs(value);
		public static float Abs(float value) => Math.Abs(value);
		public static long Abs(long value) => Math.Abs(value);
		public static int Abs(int value) => Math.Abs(value);
		public static short Abs(short value) => Math.Abs(value);
		public static sbyte Abs(sbyte value) => Math.Abs(value);
		public static decimal Abs(decimal value) => Math.Abs(value);

		public static int Sign(double value) => Math.Sign(value);
		public static int Sign(float value) => Math.Sign(value);
		public static int Sign(long value) => Math.Sign(value);
		public static int Sign(int value) => Math.Sign(value);
		public static int Sign(short value) => Math.Sign(value);
		public static int Sign(sbyte value) => Math.Sign(value);
		public static int Sign(decimal value) => Math.Sign(value);

		public static double Max(double val1, double val2) => Math.Max(val1, val2);
		public static float Max(float val1, float val2) => Math.Max(val1, val2);
		public static long Max(long val1, long val2) => Math.Max(val1, val2);
		public static int Max(int val1, int val2) => Math.Max(val1, val2);
		public static short Max(short val1, short val2) => Math.Max(val1, val2);
		public static sbyte Max(sbyte val1, sbyte val2) => Math.Max(val1, val2);
		public static ulong Max(ulong val1, ulong val2) => Math.Max(val1, val2);
		public static uint Max(uint val1, uint val2) => Math.Max(val1, val2);
		public static ushort Max(ushort val1, ushort val2) => Math.Max(val1, val2);
		public static byte Max(byte val1, byte val2) => Math.Max(val1, val2);
		public static decimal Max(decimal val1, decimal val2) => Math.Max(val1, val2);

		public static double Min(double val1, double val2) => Math.Min(val1, val2);
		public static float Min(float val1, float val2) => Math.Min(val1, val2);
		public static long Min(long val1, long val2) => Math.Min(val1, val2);
		public static int Min(int val1, int val2) => Math.Min(val1, val2);
		public static short Min(short val1, short val2) => Math.Min(val1, val2);
		public static sbyte Min(sbyte val1, sbyte val2) => Math.Min(val1, val2);
		public static ulong Min(ulong val1, ulong val2) => Math.Min(val1, val2);
		public static uint Min(uint val1, uint val2) => Math.Min(val1, val2);
		public static ushort Min(ushort val1, ushort val2) => Math.Min(val1, val2);
		public static byte Min(byte val1, byte val2) => Math.Min(val1, val2);
		public static decimal Min(decimal val1, decimal val2) => Math.Min(val1, val2);

		public static double Clamp(double val, double min, double max) => Math.Min(Math.Max(val, min), max);
		public static float Clamp(float val, float min, float max) => Math.Min(Math.Max(val, min), max);
		public static long Clamp(long val, long min, long max) => Math.Min(Math.Max(val, min), max);
		public static int Clamp(int val, int min, int max) => Math.Min(Math.Max(val, min), max);
		public static short Clamp(short val, short min, short max) => Math.Min(Math.Max(val, min), max);
		public static sbyte Clamp(sbyte val, sbyte min, sbyte max) => Math.Min(Math.Max(val, min), max);
		public static ulong Clamp(ulong val, ulong min, ulong max) => Math.Min(Math.Max(val, min), max);
		public static uint Clamp(uint val, uint min, uint max) => Math.Min(Math.Max(val, min), max);
		public static ushort Clamp(ushort val, ushort min, ushort max) => Math.Min(Math.Max(val, min), max);
		public static byte Clamp(byte val, byte min, byte max) => Math.Min(Math.Max(val, min), max);
		public static decimal Clamp(decimal val, decimal min, decimal max) => Math.Min(Math.Max(val, min), max);

		public static double Round(double a) => Math.Round(a);
		public static double Round(double value, int digits) => Math.Round(value, digits);
		public static double Round(double value, MidpointRounding mode) => Math.Round(value, mode);
		public static double Round(double value, int digits, MidpointRounding mode) => Math.Round(value, digits, mode);
		public static float Round(float a) => (float)Math.Round(a);
		public static float Round(float value, int digits) => (float)Math.Round(value, digits);
		public static float Round(float value, MidpointRounding mode) => (float)Math.Round(value, mode);
		public static float Round(float value, int digits, MidpointRounding mode) => (float)Math.Round(value, digits, mode);
		public static decimal Round(decimal d) => Math.Round(d);
		public static decimal Round(decimal d, int decimals) => Math.Round(d, decimals);
		public static decimal Round(decimal d, MidpointRounding mode) => Math.Round(d, mode);
		public static decimal Round(decimal d, int decimals, MidpointRounding mode) => Math.Round(d, decimals, mode);

		public static double Ceiling(double d) => Math.Ceiling(d);
		public static float Ceiling(float f) => (float)Math.Ceiling(f);
		public static decimal Ceiling(decimal d) => Math.Ceiling(d);
		public static double Floor(double d) => Math.Floor(d);
		public static float Floor(float f) => (float)Math.Floor(f);
		public static decimal Floor(decimal d) => Math.Floor(d);
		public static double Truncate(double d) => Math.Truncate(d);
		public static float Truncate(float f) => (float)Math.Truncate(f);
		public static decimal Truncate(decimal d) => Math.Truncate(d);

		public static double Ceil(double d) => Math.Ceiling(d);
		public static float Ceil(float f) => (float)Math.Ceiling(f);
		public static decimal Ceil(decimal d) => Math.Ceiling(d);
		public static double Trunc(double d) => Math.Truncate(d);
		public static float Trunc(float f) => (float)Math.Truncate(f);
		public static decimal Trunc(decimal d) => Math.Truncate(d);

		public static int RoundToInt(double d) => (int)Math.Round(d);
		public static int CeilToInt(double d) => (int)Math.Ceiling(d);
		public static int FloorToInt(double d) => (int)Math.Floor(d);
		public static int TruncToInt(double d) => (int)Math.Truncate(d);

		public static double Sin(double a) => Math.Sin(a);
		public static double Cos(double a) => Math.Cos(a);
		public static double Tan(double a) => Math.Tan(a);
		public static double Sinh(double a) => Math.Sinh(a);
		public static double Cosh(double a) => Math.Cosh(a);
		public static double Tanh(double a) => Math.Tanh(a);
		public static double Acos(double r) => Math.Acos(r);
		public static double Asin(double r) => Math.Asin(r);
		public static double Atan(double r) => Math.Atan(r);
		public static double Atan2(double y, double x) => Math.Atan2(y, x);

		public static float Sin(float a) => (float)Math.Sin(a);
		public static float Cos(float a) => (float)Math.Cos(a);
		public static float Tan(float a) => (float)Math.Tan(a);
		public static float Sinh(float a) => (float)Math.Sinh(a);
		public static float Cosh(float a) => (float)Math.Cosh(a);
		public static float Tanh(float a) => (float)Math.Tanh(a);
		public static float Acos(float r) => (float)Math.Acos(r);
		public static float Asin(float r) => (float)Math.Asin(r);
		public static float Atan(float r) => (float)Math.Atan(r);
		public static float Atan2(float y, float x) => (float)Math.Atan2(y, x);

		public static double Sqrt(double d) => Math.Sqrt(d);
		public static long BigMul(int a, int b) => Math.BigMul(a, b);
		public static int DivRem(int a, int b, out int result) => Math.DivRem(a, b, out result);
		public static long DivRem(long a, long b, out long result) => Math.DivRem(a, b, out result);
		public static double Exp(double d) => Math.Exp(d);
		public static double IEEERemainder(double x, double y) => Math.IEEERemainder(x, y);
		public static double Log(double a, double newBase) => Math.Log(a, newBase);
		public static double Log(double d) => Math.Log(d);
		public static double Log10(double d) => Math.Log10(d);
		public static double Pow(double x, double y) => Math.Pow(x, y);
	}
}
