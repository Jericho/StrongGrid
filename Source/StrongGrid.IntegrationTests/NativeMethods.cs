using System;
using System.Runtime.InteropServices;

namespace StrongGrid.IntegrationTests
{
	public static class NativeMethods
	{
		public const Int32 MONITOR_DEFAULT_TO_PRIMARY = 0x00000001;
		public const Int32 MONITOR_DEFAULT_TO_NEAREST = 0x00000002;

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr GetConsoleWindow();

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool GetWindowRect(IntPtr hWnd, out NativeRectangle rc);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool MoveWindow(IntPtr hWnd, int x, int y, int w, int h, bool repaint);

		[DllImport("user32.dll")]
		public static extern IntPtr MonitorFromWindow(IntPtr handle, Int32 flags);

		[DllImport("user32.dll")]
		public static extern Boolean GetMonitorInfo(IntPtr hMonitor, NativeMonitorInfo lpmi);

		[Serializable, StructLayout(LayoutKind.Sequential)]
		public struct NativeRectangle
		{
			public Int32 Left;
			public Int32 Top;
			public Int32 Right;
			public Int32 Bottom;

			public NativeRectangle(Int32 left, Int32 top, Int32 right, Int32 bottom)
			{
				this.Left = left;
				this.Top = top;
				this.Right = right;
				this.Bottom = bottom;
			}
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public sealed class NativeMonitorInfo
		{
			public Int32 Size = Marshal.SizeOf(typeof(NativeMonitorInfo));
			public NativeRectangle Monitor;
			public NativeRectangle Work;
			public Int32 Flags;
		}
	}
}
