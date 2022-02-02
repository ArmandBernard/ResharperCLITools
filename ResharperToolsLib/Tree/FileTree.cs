using System.Collections.Generic;
using System.IO;
using System.Linq;

#nullable enable

namespace ResharperToolsLib.Tree
{
    public static class FileTree
    {
        public static IList<ITreeItem> GetTreeItems(string path)
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

        public static IList<ITreeItem> MergeTrees(IList<ITreeItem> mergeInto, IList<ITreeItem> mergeWith)
        {
            var merged = new List<ITreeItem>();

            foreach (var item in mergeInto)
            {
                if (item is FileItem fItem)
                {
                    // look for same file in mergedWith
                    var mw = (FileItem?)mergeWith.FirstOrDefault(i => i.Path == item.Path);

                    // add the item with checked value from mergeWith if found
                    merged.Add(new FileItem(item.Name, item.Path) { IsChecked = mw != null ? mw.IsChecked : false });
                }
                else if (item is DirectoryItem dItem)
                {
                    // look for same directory in mergedWith
                    var mw = (DirectoryItem?)mergeWith.FirstOrDefault(i => i.Path == item.Path);

                    // merge items from each directory
                    merged.Add(new DirectoryItem(dItem.Name, dItem.Path) { Items = MergeTrees(dItem.Items, mw != null ? mw.Items : new List<ITreeItem>()) });
                }
            }

            return merged;
        }
    }
}