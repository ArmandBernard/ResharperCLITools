using System.Collections.Generic;
using NUnit.Framework;
using ResharperToolsLib.Tree;

namespace ResharperToolsLibTests
{
    internal class FileTreeTests
    {
        private IList<ITreeItem> Tree1 { get; } = new ITreeItem[]
        {
            new DirectoryItem("BaseDirectory", ".\\BaseDirectory")
            {
                Items = new[]
                {
                    new FileItem("File1", ".\\BaseDirectory\\File1")
                }
            }
        };

        private IList<ITreeItem> Tree2 { get; } = new ITreeItem[]
        {
            new DirectoryItem("BaseDirectory", ".\\BaseDirectory")
            {
                Items = new[]
                {
                    new FileItem("File2", ".\\BaseDirectory\\File2")
                }
            }
        };

        [Test]
        public void MergeTrees_TwoDifferentTrees_ReturnsMerged()
        {
            var merged = FileTree.MergeTrees(Tree1, Tree2);

            Assert.IsTrue(merged.Count == 1, "Expected one tree item at root level");
            Assert.IsAssignableFrom<DirectoryItem>(merged[0]);
            var dir = (DirectoryItem) merged[0];
            Assert.IsTrue(dir.Items.Count == 2, "Expected two files inside first directory");
        }
    }
}