using System;

#nullable enable

namespace ResharperCLIToolsGUI.Models
{
    public class Solution
    {
        public Solution(string path, DateTime lastOpened)
        {
            Path = path;
            LastOpened = lastOpened;
        }

        public string Path { get; set; }

        public DateTime LastOpened { get; set; }
    }
}
