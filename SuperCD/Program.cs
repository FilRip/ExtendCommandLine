using System;
using System.Diagnostics;

using SuperCD.Models;

using static ExtendCommandLineLib.ExtensionsCommandLineArguments;

namespace SuperCD
{
    internal static class Program
    {
        internal static void Main()
        {
            Treatment treatment = new();
            if (ArgumentPresent("-scan"))
            {
                treatment.MakeAScan();
            }
            else if (ArgumentPresent("-remove"))
            {
                treatment.Remove();
            }
            else
            {
                treatment.Search();
            }
            treatment.CloseDatabase();
        }

        internal static void ChangeDirectory(string directory)
        {
            Process parentProcess = ParentProcessUtilities.GetParentProcess();
            if (parentProcess != null)
            {
                string changeDrive = "";
                if (parentProcess.ProcessName == "cmd")
                    changeDrive = " /D";
                SendCommand(parentProcess.MainWindowHandle, $"cd{changeDrive} \"" + directory + "\"");
            }
        }

        internal static void SendCommand(IntPtr windowHandle, string command)
        {
            foreach (char key in command)
                SendMessageToWindow.PostMessage(windowHandle, 0x102, key, IntPtr.Zero);
            SendMessageToWindow.PostMessage(windowHandle, 0x100, 0x0D, IntPtr.Zero);
        }
    }
}
