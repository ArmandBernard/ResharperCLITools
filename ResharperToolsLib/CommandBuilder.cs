using System.Collections.Generic;
using System.Text;

namespace ResharperToolsLib
{
    public class CommandBuilder
    {
        public string Solution { get; }

        public CommandBuilder(string solution)
        {
            Solution = solution;
        }

        public string Clean(IList<string> files = null)
        {
            var commandBuilder = new StringBuilder();

            commandBuilder.Append($"CleanUpCode \"{Solution}\"");

            if (files == null)
            {
                return commandBuilder.ToString();
            }

            commandBuilder.Append(" --include=\"");

            var joinedPaths = string.Join(";", files);

            commandBuilder.Append(joinedPaths);

            commandBuilder.Append("\"");

            return commandBuilder.ToString();
        }
    }
}