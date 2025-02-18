using System;
using System.Runtime.InteropServices;

namespace WebView2TouchDemo
{
    public class TouchInjector
    {
        [DllImport("user32.dll")]
        static extern bool InitializeTouchInjection(uint maxCount, TouchFeedback feedbackMode);

        [DllImport("user32.dll")]
        static extern bool InjectTouchInput(uint count, [MarshalAs(UnmanagedType.LPArray), In] POINTER_TOUCH_INFO[] contacts);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINTER_TOUCH_INFO
        {
            public POINTER_INFO pointerInfo;
            public TouchFlags touchFlags;
            public TouchMask touchMask;
            public int rcContact_left;
            public int rcContact_top;
            public int rcContact_right;
            public int rcContact_bottom;
            public int rcContactRaw_left;
            public int rcContactRaw_top;
            public int rcContactRaw_right;
            public int rcContactRaw_bottom;
            public uint pressure;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINTER_INFO
        {
            public PointerInputType pointerType;
            public uint pointerId;
            public uint frameId;
            public PointerFlags pointerFlags;
            public IntPtr sourceDevice;
            public IntPtr hwndTarget;
            public POINT ptPixelLocation;
            public POINT ptHimetricLocation;
            public POINT ptPixelLocationRaw;
            public POINT ptHimetricLocationRaw;
            public uint dwTime;
            public uint historyCount;
            public int inputData;
            public uint dwKeyStates;
            public ulong PerformanceCount;
            public PointerButtonChangeType ButtonChangeType;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public enum TouchFeedback
        {
            DEFAULT = 0x1,
            INDIRECT = 0x2,
            NONE = 0x3
        }

        public enum TouchFlags
        {
            NONE = 0x00000000
        }

        public enum TouchMask
        {
            NONE = 0x00000000,
            CONTACTAREA = 0x00000001,
            ORIENTATION = 0x00000002,
            PRESSURE = 0x00000004
        }

        public enum PointerFlags
        {
            NONE = 0x00000000,
            NEW = 0x00000001,
            INRANGE = 0x00000002,
            INCONTACT = 0x00000004,
            FIRSTBUTTON = 0x00000010,
            SECONDBUTTON = 0x00000020,
            THIRDBUTTON = 0x00000040,
            PRIMARYEFFECTOR = 0x00000100,
            DOWN = 0x00010000,
            UPDATE = 0x00020000,
            UP = 0x00040000
        }

        public enum PointerInputType
        {
            POINTER = 0x00000001,
            TOUCH = 0x00000002,
            PEN = 0x00000003,
            MOUSE = 0x00000004
        }

        public enum PointerButtonChangeType
        {
            NONE,
            FIRSTBUTTON_DOWN,
            FIRSTBUTTON_UP,
            SECONDBUTTON_DOWN,
            SECONDBUTTON_UP,
            THIRDBUTTON_DOWN,
            THIRDBUTTON_UP,
            FOURTHBUTTON_DOWN,
            FOURTHBUTTON_UP,
            FIFTHBUTTON_DOWN,
            FIFTHBUTTON_UP
        }

        public static void SimulateTouch(int x, int y)
        {
            InitializeTouchInjection(1, TouchFeedback.DEFAULT);

            var contact = new POINTER_TOUCH_INFO();

            // Set pointer info
            contact.pointerInfo.pointerType = PointerInputType.TOUCH;
            contact.pointerInfo.pointerId = 0;
            contact.pointerInfo.ptPixelLocation.x = x;
            contact.pointerInfo.ptPixelLocation.y = y;
            contact.pointerInfo.pointerFlags = PointerFlags.DOWN | PointerFlags.INRANGE | PointerFlags.INCONTACT;

            // Set touch flags
            contact.touchFlags = TouchFlags.NONE;
            contact.touchMask = TouchMask.NONE;

            // Contact area
            contact.rcContact_left = x - 2;
            contact.rcContact_right = x + 2;
            contact.rcContact_top = y - 2;
            contact.rcContact_bottom = y + 2;

            var contacts = new POINTER_TOUCH_INFO[] { contact };

            // Inject touch down
            InjectTouchInput(1, contacts);

            // Change to touch up
            contact.pointerInfo.pointerFlags = PointerFlags.UP;
            InjectTouchInput(1, contacts);
        }
        public static void SimulateTouchInWindow(IntPtr windowHandle, int x, int y)
        {
            RECT windowRect;
            if (GetWindowRect(windowHandle, out windowRect))
            {
                // Add window position to coordinates
                int screenX = x + windowRect.Left;
                int screenY = y + windowRect.Top;
                SimulateTouch(screenX, screenY);
            }
        }
    }
}