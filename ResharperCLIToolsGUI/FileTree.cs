using ResharperCLIToolsGUI.Tree;
using System.Collections.Generic;
using System.IO;

namespace ResharperCLIToolsGUI
{
    public static partial class FileTree
    {
        public static List<ITreeItem> GetTreeItems(string path)
        {
            var items = new List<ITreeItem>();

            var dirInfo = new DirectoryInfo(path);

            foreach (var directory in dirInfo.GetDirectories())
            {
                var item = new DirectoryItem(directory.Name, directory.FullName)
                {
                    Items = GetTreeItems(directory.FullName)
                };

                items.Add(item);
            }

            foreach (var file in dirInfo.GetFiles())
            {
                var item = new FileItem(file.Name, file.FullName);

                items.Add(item);
            }

            return items;
        }
    }
    
}
