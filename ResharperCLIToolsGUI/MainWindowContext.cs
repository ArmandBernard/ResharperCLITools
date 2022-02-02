using ResharperToolsLib.Tree;
using System.Collections.Generic;

namespace ResharperCLIToolsGUI
{
    public class MainWindowContext
    {
        public IList<ITreeItem> TreeItems { get; set; }

        public MainWindowContext()
        {
            TreeItems = new List<ITreeItem>();
        }
    }
}