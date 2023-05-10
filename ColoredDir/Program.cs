using System;
using System.IO;
using System.Collections.Generic;

using ColoredDir.Modeles;
using System.Linq;

using static ExtendCommandLineLib.ExtensionsCommandLineArguments;

using ExtendCommandLineLib;

namespace ColoredDir
{
    internal static class Program
    {
#pragma warning disable S2223 // Non-constant static fields should not be visible
        internal static ConsoleColor _defaultColor;
#pragma warning restore S2223 // Non-constant static fields should not be visible

        internal static void Main(string[] args)
        {
            _defaultColor = Console.ForegroundColor;
            string path = Environment.CurrentDirectory;
            if (args != null)
            {
                string s = args.FirstOrDefault(str => !string.IsNullOrWhiteSpace(str) && Directory.Exists(str));
                if (!string.IsNullOrWhiteSpace(s))
                    path = s;
            }

            if (ArgumentPresent("/s"))
            {
                DirRecursive(path, out int nbDirectory, out int nbFiles, out long totalSize);
                Console.WriteLine("");
                Console.WriteLine($"Total for all directory : {nbFiles} file(s), {nbDirectory} directory(ies)");
                Console.WriteLine($"Total size : {totalSize.ToNumberFormat()}");
            }
            else
                DirOneDirectory(path, out _, out _, out _);
            Console.ForegroundColor = _defaultColor;
        }

        internal static void DirRecursive(string path, out int nbDirectory, out int nbFiles, out long totalSize)
        {
            DirOneDirectory(path, out int nbDir, out int nbFile, out long size);
            nbDirectory = nbDir;
            nbFiles = nbFile;
            totalSize = size;
            foreach (string dir in Directory.GetDirectories(path)?.OrderBy(s => s))
            {
                DirRecursive(dir, out int nbdir, out int nbfile, out long totsize);
                nbDirectory += nbdir;
                nbFiles += nbfile;
                totalSize += totsize;
            }
        }

        internal static void DirOneDirectory(string path, out int nbDirectory, out int nbFiles, out long totalSize)
        {
            Console.ForegroundColor = _defaultColor;
            Console.WriteLine("");
            Console.WriteLine($"Directory : {path}");
            Console.WriteLine("");
            List<OneFileSystem> listElements = new();
            OneFileSystem element;
            FileInfo fi;
            listElements.Add(new OneFileSystem() { Name = ".", FullPath = path + Path.DirectorySeparatorChar + ".", Attributes = File.GetAttributes(path), LastWrite = File.GetLastWriteTime(path), Order = 0 });
            listElements.Add(new OneFileSystem() { Name = "..", FullPath = path + Path.DirectorySeparatorChar + "..", Attributes = File.GetAttributes(path), LastWrite = File.GetLastWriteTime(path), Order = 0 });
            foreach (string dir in Directory.GetDirectories(path))
            {
                element = new OneFileSystem()
                {
                    FullPath = dir,
                    Name = Path.GetFileName(dir),
                    Attributes = File.GetAttributes(dir),
                    LastWrite = File.GetLastWriteTime(dir),
                    Order = 0,
                };
                listElements.Add(element);
            }
            nbDirectory = listElements.Count;
            nbFiles = 0;
            totalSize = 0;
            foreach (string file in Directory.GetFiles(path))
            {
                element = new OneFileSystem()
                {
                    FullPath = file,
                    Name = Path.GetFileName(file),
                    Attributes = File.GetAttributes(file),
                    LastWrite = File.GetLastWriteTime(file),
                    Order = 1,
                };
                fi = new FileInfo(file);
                element.Size = fi.Length;
                totalSize += fi.Length;
                nbFiles++;
                listElements.Add(element);
            }
            if (listElements.Count > 0)
            {
                listElements = listElements.OrderBy(item => item.Order).ThenBy(item => item.Name).ToList();
                int greaterSize = listElements.Max(item => item.Size).ToNumberFormat().Length;
                foreach (OneFileSystem item in listElements)
                {
                    item.WriteToConsole(greaterSize);
                }
                Console.ForegroundColor = _defaultColor;
                Console.WriteLine("");
                Console.WriteLine($"Total : {listElements.Count(item => item.IsFile)} file(s), {listElements.Count(item => item.IsDirectory)} directory(ies)");
                Console.WriteLine($"Total size : {listElements.Sum(item => item.Size).ToNumberFormat()}");
            }
            else
            {
                Console.ForegroundColor = _defaultColor;
                Console.WriteLine("Total : 0 file, 0 directory");
            }
        }
    }
}
