using System.Collections.Generic;
using NUnit.Framework;
using ResharperToolsLib.Tree;

namespace ResharperToolsLibTests
{
    internal class FileTreeTests
    {
        [Test]
        public void ImportCheckState_NewFileInTree_ReturnsTreeWithNewFile()
        {
            // Arrange
            var oldTree = new ITreeItem[]
            {
                new DirectoryItem("BaseDirectory", ".\\BaseDirectory")
                {
                    Items = new[]
                    {
                        new FileItem("File1", ".\\BaseDirectory\\File1")
                    }
                }
            };

            var updatedTree = new ITreeItem[]
            {
                new DirectoryItem("BaseDirectory", ".\\BaseDirectory")
                {
                    Items = new[]
                    {
                        new FileItem("File1", ".\\BaseDirectory\\File1"),
                        new FileItem("File2", ".\\BaseDirectory\\File2")
                    }
                }
            };

            var merged = FileTree.ImportCheckState(updatedTree, oldTree);

            Assert.IsTrue(merged.Count == 1, "Expected one tree item at root level");
            Assert.IsAssignableFrom<DirectoryItem>(merged[0]);
            var dir = (DirectoryItem) merged[0];
            Assert.IsTrue(dir.Items.Count == 2, "Expected two files inside first directory");
        }

        [Test]
        public void ImportCheckState_RenamedFile_ReturnsCorrectFileOnly()
        {
            // Arrange
            var oldTree = new ITreeItem[]
            {
                new DirectoryItem("BaseDirectory", ".\\BaseDirectory")
                {
                    Items = new[]
                    {
                        new FileItem("File1", ".\\BaseDirectory\\File1")
                    }
                }
            };

            var newFileName = "File2";

            var updatedTree = new ITreeItem[]
            {
                new DirectoryItem("BaseDirectory", ".\\BaseDirectory")
                {
                    Items = new[]
                    {
                        new FileItem(newFileName, ".\\BaseDirectory\\File2")
                    }
                }
            };

            var merged = FileTree.ImportCheckState(updatedTree, oldTree);

            Assert.IsTrue(merged.Count == 1, "Expected one tree item at root level");
            Assert.IsAssignableFrom<DirectoryItem>(merged[0]);
            var dir = (DirectoryItem)merged[0];
            Assert.IsTrue(dir.Items.Count == 1, "Expected one file inside first directory");
            Assert.IsAssignableFrom<FileItem>(dir.Items[0]);
            var file = (FileItem)dir.Items[0];
            Assert.AreEqual(file.Name, newFileName);
        }

        [Test]
        public void ImportCheckState_FileWasChecked_CheckStateImported()
        {
            // Arrange
            var oldTree = new ITreeItem[]
            {
                new DirectoryItem("BaseDirectory", ".\\BaseDirectory")
                {
                    Items = new[]
                    {
                        new FileItem("File1", ".\\BaseDirectory\\File1") {IsChecked = true}
                    }
                }
            };

            var updatedTree = new ITreeItem[]
            {
                new DirectoryItem("BaseDirectory", ".\\BaseDirectory")
                {
                    Items = new[]
                    {
                        new FileItem("File1", ".\\BaseDirectory\\File1") {IsChecked = false}
                    }
                }
            };

            var merged = FileTree.ImportCheckState(updatedTree, oldTree);

            Assert.IsTrue(merged.Count == 1, "Expected one tree item at root level");
            Assert.IsAssignableFrom<DirectoryItem>(merged[0]);
            var dir = (DirectoryItem)merged[0];
            Assert.IsTrue(dir.Items.Count == 1, "Expected one file inside first directory");
            Assert.IsAssignableFrom<FileItem>(dir.Items[0]);
            var file = (FileItem)dir.Items[0];
            Assert.IsTrue(file.IsChecked, "Expected file to be checked");
        }
    }
}