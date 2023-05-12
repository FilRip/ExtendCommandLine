using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SuperCD.Models
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct ParentProcessUtilities
    {
        // These members must match PROCESS_BASIC_INFORMATION
        internal IntPtr Reserved1;
        internal IntPtr PebBaseAddress;
        internal IntPtr Reserved2_0;
        internal IntPtr Reserved2_1;
        internal IntPtr UniqueProcessId;
        internal IntPtr InheritedFromUniqueProcessId;

        [DllImport("ntdll.dll")]
        private static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass, ref ParentProcessUtilities processInformation, int processInformationLength, out int returnLength);

        public static Process GetParentProcess()
        {
            ParentProcessUtilities pbi = new();
            int status = NtQueryInformationProcess(Process.GetCurrentProcess().Handle, 0, ref pbi, Marshal.SizeOf(pbi), out _);
            if (status == 0)
                return Process.GetProcessById(pbi.InheritedFromUniqueProcessId.ToInt32());
            else
                return null;
        }
    }
}
