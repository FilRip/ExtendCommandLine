﻿using SuperCD.Models;

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
        }
    }
}