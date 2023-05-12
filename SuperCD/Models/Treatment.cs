using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

using static ExtendCommandLineLib.ExtensionsCommandLineArguments;

namespace SuperCD.Models
{
    internal class Treatment
    {
        private readonly SQLiteOperations _databaseInteraction;
        private readonly bool _includeHiddenDirectory;

        public Treatment()
        {
            _databaseInteraction = new();
            _includeHiddenDirectory = ArgumentPresent("-withhidden");
        }

        public void Search()
        {
            List<string> dirToSearch = Arguments().Split(' ').ToList();
            if (dirToSearch.Count > 0 )
            {
                if (dirToSearch.Count == 1)
                {
                    DataTable oneShot = _databaseInteraction.Result($"select fullpath from scan where name='{dirToSearch[0]}'");
                    if (oneShot.Rows.Count == 1)
                    {
                        Program.ChangeDirectory(oneShot.Rows[0][0].ToString());
                        return;
                    }
                }
                StringBuilder query = new("select FullPath, Name from scan where 1=1");
                foreach (string s in dirToSearch )
                {
                    query.Append($" AND lower(Name) like '%{s.ToLower()}%'");
                }
                DataTable result = _databaseInteraction.Result(query.ToString());
                if (result.Rows.Count == 0)
                    Console.WriteLine("No match found");
                else if (result.Rows.Count == 1)
                {
                    Program.ChangeDirectory(result.Rows[0][0].ToString());
                }
                else
                {
                    if (result.Rows.Count > 9)
                        Console.WriteLine("Too many matches");
                    else
                    {
                        Console.WriteLine("Found :");
                        int i = 0;
                        foreach (DataRow row in result.Rows)
                        {
                            i++;
                            Console.WriteLine(i.ToString() + "= " + row[0].ToString());
                        }
                        Console.WriteLine("Enter number of your choice, or ENTER to cancel");
                        ConsoleKeyInfo numPressed = Console.ReadKey();
                        if (!string.IsNullOrWhiteSpace(numPressed.KeyChar.ToString()) &&
                            int.TryParse(numPressed.KeyChar.ToString(), out int index) &&
                            index > 0 && index <= result.Rows.Count)
                        {
                            Program.ChangeDirectory(result.Rows[index - 1][0].ToString());
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
                    if (s.ToLower().Trim() != "-scan" && s.ToLower().Trim() != "-withhidden")
                        dirToScan.Add(Path.GetFullPath(s));
                });
            }
            if (dirToScan.Count == 0)
                dirToScan.Add(Environment.CurrentDirectory.Substring(0,3));
            Console.WriteLine("Scanning...");
            foreach (string s in dirToScan)
            {
                string path = Path.GetFullPath(s);
                Remove(path);
                ScanDirRecursively(path);
            }
            Console.WriteLine("Done!");
        }

        private void ScanDirRecursively(string path)
        {
            string[] listSubDir = null;
            try
            {
                if (!_includeHiddenDirectory && path.Length > 3 && File.GetAttributes(path).HasFlag(FileAttributes.Hidden))
                    return;
                _databaseInteraction.Insert(path, Path.GetFileName(path));
                listSubDir = Directory.GetDirectories(path);
            }
            catch (Exception) { /* Ignore errors, probably current user has not enought rights */ }
            if (listSubDir != null)
                foreach (string subDir in listSubDir)
                {
                    if (!_includeHiddenDirectory && File.GetAttributes(subDir).HasFlag(FileAttributes.Hidden))
                        continue;
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
                _databaseInteraction.RemoveSubDir(Path.GetFullPath(s));
            Console.WriteLine("Done!");
        }

        private void Remove(string s)
        {
            _databaseInteraction.RemoveSubDir(Path.GetFullPath(s));
        }

        public void CloseDatabase()
        {
            _databaseInteraction?.Close();
        }
    }
}
