using System.Collections.Generic;
using System.IO;
using System;
using ExtendCommandLineLib;
using System.Linq;

namespace ColoredDir.Models
{
    internal class Treatment
    {
        private readonly Config _config;

        public Treatment(Config config)
        {
            _config = config;
        }

        public void Start()
        {
            foreach (string path in _config.DirToList)
            {
                if (_config.WithSubFolder)
                {
                    DirRecursive(path, out int nbDirectory, out int nbFiles, out long totalSize);
                    if (!_config.NoTitleNoSummary)
                    {
                        Program.WriteToConsole("");
                        Program.WriteToConsole($"Total for all directory : {nbFiles} file(s), {nbDirectory} directory(ies)");
                        Program.WriteToConsole($"Total size : {totalSize.ToNumberFormat()}");
                    }
                }
                else
                    DirOneDirectory(path, out _, out _, out _);
            }

            Console.ForegroundColor = _config.DefaultColor;
            DriveInfo currentDrive = new(_config.DirToList[0]);
            if (!_config.NoTitleNoSummary)
            {
                Program.WriteToConsole("");
                Program.WriteToConsole($"Free space on {currentDrive.VolumeLabel}({currentDrive.RootDirectory.Name.Replace("\\", "")}) : {currentDrive.AvailableFreeSpace.ToNumberFormat()}");
            }
        }

        internal void DirRecursive(string path, out int nbDirectory, out int nbFiles, out long totalSize)
        {
            DirOneDirectory(path, out int nbDir, out int nbFile, out long size);
            nbDirectory = nbDir;
            nbFiles = nbFile;
            totalSize = size;
            foreach (string dir in Directory.GetDirectories(path, _config.Pattern)?.OrderBy(s => s))
            {
                DirRecursive(dir, out int nbdir, out int nbfile, out long totsize);
                nbDirectory += nbdir;
                nbFiles += nbfile;
                totalSize += totsize;
            }
        }

        internal void DirOneDirectory(string path, out int nbDirectory, out int nbFiles, out long totalSize)
        {
            Console.ForegroundColor = _config.DefaultColor;
            if (!_config.NoTitleNoSummary)
            {
                Program.WriteToConsole("");
                Program.WriteToConsole($"Directory : {path}");
                Program.WriteToConsole("");
            }
            List<OneFileSystem> listElements = new();
            OneFileSystem element;
            FileInfo fi;
            if (string.IsNullOrWhiteSpace(_config.Pattern))
            {
                listElements.Add(new OneFileSystem() { Name = ".", FullPath = path + Path.DirectorySeparatorChar + ".", Attributes = File.GetAttributes(path), LastWrite = File.GetLastWriteTime(path), Order = 0 });
                listElements.Add(new OneFileSystem() { Name = "..", FullPath = path + Path.DirectorySeparatorChar + "..", Attributes = File.GetAttributes(path), LastWrite = File.GetLastWriteTime(path), Order = 0 });
            }
            foreach (string dir in Directory.GetDirectories(path, _config.Pattern))
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
            nbDirectory = 0;
            nbFiles = 0;
            totalSize = 0;
            foreach (string file in Directory.GetFiles(path, _config.Pattern))
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
                listElements.Add(element);
            }
            if (listElements.Count > 0)
            {
                listElements = listElements.OrderBy(item => item.Order).ThenBy(item => item.Name).ToList();
                int greaterSize = listElements.Max(item => item.Size).ToNumberFormat().Length;
                foreach (OneFileSystem item in listElements.Where(i => i.WriteToConsole(greaterSize, _config)))
                {
                    if (item.IsFile)
                    {
                        nbFiles++;
                        totalSize += item.Size;
                    }
                    else
                        nbDirectory++;
                }
                Console.ForegroundColor = _config.DefaultColor;
                if (!_config.NoTitleNoSummary)
                {
                    Program.WriteToConsole("");
                    Program.WriteToConsole($"Total : {nbFiles} file(s), {nbDirectory} directory(ies)");
                    Program.WriteToConsole($"Total size : {totalSize.ToNumberFormat()}");
                }
            }
            else
            {
                Console.ForegroundColor = _config.DefaultColor;
                if (!_config.NoTitleNoSummary)
                    Program.WriteToConsole("Total : 0 file, 0 directory");
            }
        }
    }
}
