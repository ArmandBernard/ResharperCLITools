﻿using System.Diagnostics;

namespace ResharperToolsLib
{
    public class CommandRunner
    {
        public void Run(string command)
        {
            Process.Start("CMD.exe", command);
        }
    }
}