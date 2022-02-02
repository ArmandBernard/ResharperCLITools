using ResharperToolsLib.Tree;
using System;
using System.Collections.Generic;

#nullable enable

namespace ResharperCLIToolsGUI.Models
{
    public record Solution(string Name, string Path, DateTime LastOpened, IList<ITreeItem> SelectedFiles);
}
