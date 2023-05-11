using System;
using System.Collections.Generic;

namespace ColoredDir.Modeles
{
    internal class Config
    {
        public Config()
        {
            DirToList = new List<string>();
        }

        public ConsoleColor DefaultColor { get; set; }

        public List<string> DirToList { get; set; }

        public string Pattern { get; set; }

        public bool ShowHidden { get; set; }

        public bool WithSubFolder { get; set; }

        public bool NoTitleNoSummary { get; set; }

        public bool ShowOnlyReadOnly { get; set; }

        public bool ShowOnlyDirectory { get; set; }

        public bool LowerCase { get; set; }

        public bool ShowShortName { get; set; }

        public bool ShowOwner { get; set; }
    }
}
