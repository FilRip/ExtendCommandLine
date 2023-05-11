using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using static ExtendCommandLineLib.ExtensionsCommandLineArguments;

namespace SuperCD.Models
{
    internal class Treatment
    {
        private readonly SQLiteOperations _databaseInteraction;

        public Treatment()
        {
            _databaseInteraction = new();
        }

        public void Search()
        {
            List<string> dirToSearch = Arguments().Split(' ').ToList();
            if (dirToSearch.Count > 0 )
            {
                StringBuilder query = new("select FullPath, Name from scan where 1=1");
                foreach (string s in dirToSearch )
                {
                    query.Append($" AND Name like '%{s}%'");
                }
                DataTable result = _databaseInteraction.Result(query.ToString());
                if (result.Rows.Count == 0)
                    Console.WriteLine("No match found");
                else if (result.Rows.Count == 1)
                {
                    string toSend = "cd \"" + result.Rows[0][0].ToString() + "\"{ENTER}";
                    SendKeys.SendWait(toSend);
                }
                else
                {
                    if (result.Rows.Count > Console.WindowHeight - 2)
                        Console.WriteLine("Too many matches");
                    else
                    {
                        Console.WriteLine("Found :");
                        foreach (DataRow row in result.Rows)
                        {
                            Console.WriteLine(row[0]);
                        }
                    }
                }
            }
        }

        public void MakeAScan()
        {
            List<string> dirToScan = new();
            string[] listArgs = Arguments().Split(' ');
            if (listArgs.Length > 1)
            {
                listArgs.ToList().ForEach(s =>
                {
                    if (s.ToLower().Trim() != "-scan")
                        dirToScan.Add(s);
                });
            }
            if (dirToScan.Count == 0)
                dirToScan.Add(Environment.CurrentDirectory.Substring(0,3));
            Console.WriteLine("Scanning...");
            foreach (string s in dirToScan)
            {
                Remove(s);
                ScanDirRecursively(s);
            }
            Console.WriteLine("Done!");
        }

        private void ScanDirRecursively(string path)
        {
            string[] listSubDir = null;
            try
            {
                _databaseInteraction.Insert(path, Path.GetFileName(path));
                listSubDir = Directory.GetDirectories(path);
            }
            catch (Exception) { /* Ignore errors, probably current user has not enought rights*/ }
            if (listSubDir != null)
                foreach (string subDir in listSubDir)
                {
                    ScanDirRecursively(subDir);
                }
        }

        public void Remove()
        {
            List<string> dirToRemove = new();
            string[] listArgs = Arguments().Split(' ');
            if (listArgs.Length > 1)
            {
                listArgs.ToList().ForEach(s =>
                {
                    if (s.ToLower().Trim() != "-remove")
                        dirToRemove.Add(s);
                });
            }
            if (dirToRemove.Count == 0)
                dirToRemove.Add(Environment.CurrentDirectory);
            Console.WriteLine("Removing...");
            foreach (string s in dirToRemove)
                _databaseInteraction.RemoveSubDir(s);
            Console.WriteLine("Done!");
        }

        private void Remove(string s)
        {
            _databaseInteraction.RemoveSubDir(s);
        }
    }
}
