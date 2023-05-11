using System;
using System.IO;

using ColoredDir.Modeles;
using System.Linq;

using static ExtendCommandLineLib.ExtensionsCommandLineArguments;

using ExtendCommandLineLib;
using System.Runtime.InteropServices;
using System.Text;

namespace ColoredDir
{
    internal static class Program
    {
        internal static void Main()
        {
            Config myConf = new()
            {
                DefaultColor = Console.ForegroundColor,
                WithSubFolder = ArgumentPresent("/s"),
                Pattern = "*.*",
                ShowHidden = false,
                NoTitleNoSummary = ArgumentPresent("/b"),
                LowerCase = ArgumentPresent("/l"),
                ShowShortName = ArgumentPresent("/x"),
                ShowOwner = ArgumentPresent("/q"),
            };
            _pause = ArgumentPresent("/p");

            string arguments = Arguments();
            if (!string.IsNullOrWhiteSpace(arguments))
            {
                foreach (string path in arguments.Split(' ').Where(s => !s.StartsWith("/") && Directory.Exists(s)))
                {
                    myConf.DirToList.Add(new DirectoryInfo(path).FullName);
                }
                foreach (string path in arguments.Split(' ').Where(s => !s.StartsWith("/") && (File.Exists(s) || s.Contains("*"))))
                {
                    myConf.Pattern = path;
                }
            }
            if (myConf.DirToList.Count == 0)
            {
                DirectoryInfo di = new(Environment.CurrentDirectory);
                myConf.DirToList.Add(di.FullName);
            }

            if (ArgumentVariablePresent("/a"))
            {
                string listAttrib = ArgumentValeur("/a");
                if (string.IsNullOrWhiteSpace(listAttrib))
                    myConf.ShowHidden = true;
                else
                {
                    if (listAttrib.Contains("h", StringComparison.OrdinalIgnoreCase))
                        myConf.ShowHidden = true;
                    if (listAttrib.Contains("r", StringComparison.OrdinalIgnoreCase))
                        myConf.ShowOnlyReadOnly = true;
                    if (listAttrib.Contains("d", StringComparison.OrdinalIgnoreCase))
                        myConf.ShowOnlyDirectory = true;
                }
            }

            new Treatment(myConf).Start();

            Console.ForegroundColor = myConf.DefaultColor;
        }

        private static int _lineNumber;
        private static bool _pause;
        internal static void WriteToConsole(string str)
        {
            Console.WriteLine(str);
            if (_pause)
            {
                _lineNumber++;
                if (_lineNumber >= Console.WindowHeight - 2)
                {
                    _lineNumber = 0;
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern uint GetShortPathName(
            [MarshalAs(UnmanagedType.LPTStr)]
            string lpszLongPath,
            [MarshalAs(UnmanagedType.LPTStr)]
            StringBuilder lpszShortPath,
            uint cchBuffer);
    }
}
