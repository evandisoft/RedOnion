using System;
namespace Bee
{
	namespace Run
	{
		delegate Obj Create(Engine engine);
	}

	public static class Bits
	{
		#region Read Array
		public static int Int(byte[] a, int i)
		{
			return BitConverter.ToInt32(a, i);
		}
		public static uint Uint(byte[] a, int i)
		{
			return BitConverter.ToUInt32(a, i);
		}
		public static long Long(byte[] a, int i)
		{
			return BitConverter.ToInt64(a, i);
		}
		public static ulong Ulong(byte[] a, int i)
		{
			return BitConverter.ToUInt64(a, i);
		}
		public static short Short(byte[] a, int i)
		{
			return BitConverter.ToInt16(a, i);
		}
		public static ushort Ushort(byte[] a, int i)
		{
			return BitConverter.ToUInt16(a, i);
		}

		public static float Float(byte[] a, int i)
		{
			return Get(Uint(a, i));
		}
		public static double Double(byte[] a, int i)
		{
			return Get(Ulong(a, i));
		}
		#endregion

		#region Convert Bits
		public static unsafe uint Get(float v)
		{
			return *(uint*)&v;
		}
		public static unsafe ulong Get(double v)
		{
			return *(ulong*)&v;
		}

		public static unsafe float Get(uint v)
		{
			return *(float*)&v;
		}
		public static unsafe double Get(ulong v)
		{
			return *(double*)&v;
		}
		#endregion
	}
}
