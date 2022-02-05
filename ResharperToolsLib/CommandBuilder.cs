using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ResharperToolsLib
{
    public class CommandBuilder
    {
        public CommandBuilder(string solution)
        {
            Solution = solution;
            SolutionDirectory = Path.GetDirectoryName(Solution);
        }

        public string Solution { get; }

        public string SolutionDirectory { get; }

        public string Clean(IList<string> files = null)
        {
            var commandBuilder = new StringBuilder();

            commandBuilder.Append($"CleanUpCode \"{Solution}\"");

            if (files == null) return commandBuilder.ToString();

            commandBuilder.Append(" --include=\"");

            var joinedPaths = string.Join("\" --include=\"", files.Select(p =>
                // get path relative to the solution    
                Path.IsPathRooted(p) ? Path.GetRelativePath(SolutionDirectory, p) : p
            ));

            commandBuilder.Append(joinedPaths);

            commandBuilder.Append("\"");

            return commandBuilder.ToString();
        }
    }
}