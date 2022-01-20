using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ResharperToolsLib
{
    public class CommandBuilder
    {
        public FileInfo Solution { get; }

        public CommandBuilder(FileInfo solution)
        {
            Solution = solution;
        }

        public string Clean(IList<FileInfo> files = null)
        {
            var commandBuilder = new StringBuilder();

            commandBuilder.Append($"jb CleanUpCode \"{Solution.FullName}\"");

            if (files == null)
            {
                return commandBuilder.ToString();
            }

            commandBuilder.Append(" --include=\"");

            var joinedPaths = string.Join(";", files.Select(f => f.FullName));

            commandBuilder.Append(joinedPaths);

            commandBuilder.Append("\"");

            return commandBuilder.ToString();
        }
    }
}
