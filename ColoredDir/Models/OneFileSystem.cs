using System;
using System.IO;
using System.Text;

using ExtendCommandLineLib;

namespace ColoredDir.Models
{
    internal class OneFileSystem
    {
        public string Name { get; set; }

        public DateTime LastWrite { get; set; }

        public string FullPath { get; set; }

        public long Size { get; set; }

        public FileAttributes Attributes { get; set; }

        public bool IsDirectory
        {
            get { return Attributes.HasFlag(FileAttributes.Directory); }
        }

        public bool IsFile
        {
            get { return !Attributes.HasFlag(FileAttributes.Directory); }
        }

        public bool IsHidden
        {
            get { return Attributes.HasFlag(FileAttributes.Hidden); }
        }

        public bool IsReadOnly
        {
            get { return Attributes.HasFlag(FileAttributes.ReadOnly); }
        }

        public string Extension
        {
            get { return Path.GetExtension(Name); }
        }

        public bool IsWindowsExe
        {
            get
            {
                return Extension.ToLower() == ".exe" || Extension.ToLower() == ".bat" || Extension.ToLower() == ".cmd" || Extension.ToLower() == ".ps1";
            }
        }

        public bool IsSystemDirectory
        {
            get
            {
                return Name == "." || Name == "..";
            }
        }

        public bool IsWindowsLibrary
        {
            get { return Extension.ToLower() == ".dll"; }
        }

        public bool WriteToConsole(int greaterSize, Config conf)
        {
            if (IsHidden && !conf.ShowHidden)
                return false;
            if (!IsReadOnly && conf.ShowOnlyReadOnly)
                return false;
            if (!IsDirectory && conf.ShowOnlyDirectory)
                return false;

            if (IsDirectory)
            {
                if (IsHidden)
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                else
                    Console.ForegroundColor = ConsoleColor.Yellow;
            }
            else
            {
                if (IsHidden)
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                else
                {
                    if (IsWindowsExe)
                        Console.ForegroundColor = ConsoleColor.Green;
                    else if (IsWindowsLibrary)
                        Console.ForegroundColor = ConsoleColor.White;
                    else
                        Console.ForegroundColor = ConsoleColor.Gray;
                }
            }

            if (IsReadOnly)
            {
                if (IsHidden)
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                else
                    Console.ForegroundColor = ConsoleColor.Red;
            }

            StringBuilder output = new();
            if (!conf.NoTitleNoSummary)
            {
                output.Append($"{LastWrite.Day:00}/{LastWrite.Month:00}/{LastWrite.Year:0000} {LastWrite.Hour:00}:{LastWrite.Minute:00}   ");
                if (IsDirectory)
                {
                    output.Append("<DIR>   " + new string(' ', greaterSize));
                }
                else
                {
                    output.Append("        ");
                    string sizeOfFile = Size.ToNumberFormat();
                    output.Append(new string(' ', greaterSize - sizeOfFile.Length) + sizeOfFile);
                }
                if (conf.ShowShortName)
                {
                    StringBuilder shortname = new(256);
                    Program.GetShortPathName(Name, shortname, 256);
                    output.Append($" {shortname,-12}");
                }
                if (conf.ShowOwner)
                {
                    string owner = "";
                    try
                    {
                        if (IsFile)
                            owner = File.GetAccessControl(FullPath).GetOwner(typeof(System.Security.Principal.NTAccount)).Value;
                        else
                            owner = Directory.GetAccessControl(FullPath).GetOwner(typeof(System.Security.Principal.NTAccount)).Value;
                    }
                    catch (Exception) { /* Ignore errors, certainly current user not enough rights to access the file/directory */ }
                    output.Append($" {owner.ReturnStringWithMaxSize(22),-22}");
                }
                output.Append($" {(conf.LowerCase ? Name.ToLower() : Name)}");
            }
            else if (IsSystemDirectory)
                return false;
            else
                output.Append($" {FullPath}");

            Program.WriteToConsole(output.ToString());
            return true;
        }

        public int Order { get; set; }
    }
}
