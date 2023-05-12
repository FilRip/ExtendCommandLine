using System;
using System.Runtime.InteropServices;

namespace SuperCD.Models
{
    internal static class SendMessageToWindow
    {
        [DllImport("user32.dll")]
        internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        internal static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        internal static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, int wParam, IntPtr lParam);
    }
}
