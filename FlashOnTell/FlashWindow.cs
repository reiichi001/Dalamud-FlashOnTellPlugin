using System;
using System.Runtime.InteropServices;

namespace FlashOnTell
{
    public static class FlashWindow
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

        /// Stop flashing. The system restores the window to its original state.            
        public const uint FLASHW_STOP = 0;

        /// Flash the window caption.            
        public const uint FLASHW_CAPTION = 1;

        /// Flash the taskbar button.            
        public const uint FLASHW_TRAY = 2;

        /// Flash both the window caption and taskbar button.
        /// This is equivalent to setting the FLASHW_CAPTION | FLASHW_TRAY flags.            
        public const uint FLASHW_ALL = 3;

        /// Flash continuously, until the FLASHW_STOP flag is set.            
        public const uint FLASHW_TIMER = 4;

        /// Flash continuously until the window comes to the foreground.            
        public const uint FLASHW_TIMERNOFG = 12;

        [StructLayout(LayoutKind.Sequential)]
        public struct FLASHWINFO
        {

            /// The size of the structure in bytes.
            public uint cbSize;

            /// A Handle to the Window to be Flashed. The window can be either opened or minimized.
            public IntPtr hwnd;

            /// The Flash Status.                
            public uint dwFlags;

            /// The number of times to Flash the window.
            public uint uCount;

            /// The rate at which the Window is to be flashed, in milliseconds. If Zero, the function uses the default cursor blink rate.                
            public uint dwTimeout;
        }

        /// Flash the specified Window (Form) until it receives focus.            
        public static bool Flash(FLASHWINFO flashinfo)
        {
            // Make sure we're running under Windows 2000 or later
            if (Win2000OrLater)
            {
                return FlashWindowEx(ref flashinfo);
            }
            return false;
        }

        private static FLASHWINFO Create_FLASHWINFO(IntPtr handle, uint flags, uint count, uint timeout)
        {
            FLASHWINFO fi = new FLASHWINFO();
            fi.cbSize = Convert.ToUInt32(Marshal.SizeOf(fi));
            fi.hwnd = handle;
            fi.dwFlags = flags;
            fi.uCount = count;
            fi.dwTimeout = timeout;
            return fi;
        }


        /// helper methods    

        /// A boolean value indicating whether the application is running on Windows 2000 or later.
        private static bool Win2000OrLater
        {
            get { return System.Environment.OSVersion.Version.Major >= 5; }
        }

    }
}