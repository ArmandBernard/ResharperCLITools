using Microsoft.Win32;
using ResharperCLIToolsGUI.Tree;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using ResharperToolsLib;
using System.Linq;
using ResharperToolsLib.Logging;

namespace ResharperCLIToolsGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string? SavedDirectory { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowContext();
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = SavedDirectory ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "Solution File|*.sln",
                ValidateNames = true
            };

            if (openFileDialog.ShowDialog() != true)
            {
                return;
            }

            var file = new FileInfo(openFileDialog.FileName);

            SavedDirectory = file.Directory!.FullName;

            PopulateFileTree(SavedDirectory);
        }

        private void PopulateFileTree(string filepath)
        {
            List<ITreeItem> items = FileTree.GetTreeItems(filepath);

            ((MainWindowContext) DataContext).TreeItems = items;

            // shouldn't have to do this, but I can't get binding working
            treeView.ItemsSource = ((MainWindowContext) DataContext).TreeItems;
        }

        private void CleanButton_Click(object sender, RoutedEventArgs e)
        {
            var commandRunner = new CommandRunner(new Logger());

            commandRunner.EnsureToolsInstalled();

            var commandBuilder = new CommandBuilder(SavedDirectory);

            var treeItems = ((MainWindowContext) DataContext).TreeItems;

            var checkedPaths = GetFileIf(treeItems, (item) => item.IsChecked).Select(item => item.Path).ToArray();

            var command = commandBuilder.Clean(checkedPaths);

            commandRunner.Run(command);
        }

        private void CleanAllButton_Click(object sender, RoutedEventArgs e)
        {
            var commandRunner = new CommandRunner(new Logger());

            commandRunner.EnsureToolsInstalled();

            var commandBuilder = new CommandBuilder(SavedDirectory);

            var command = commandBuilder.Clean();

            commandRunner.Run(command);
        }

        /// <summary>
        /// Get all files recursively matching a condition
        /// </summary>
        /// <param name="items"></param>
        /// <param name="filePredicate"></param>
        /// <returns></returns>
        private IList<ITreeItem> GetFileIf(IList<ITreeItem> items, Predicate<FileItem> filePredicate)
        {
            List<ITreeItem> itemsN = new List<ITreeItem>();

            foreach (var item in items)
            {
                if (item is DirectoryItem dirItem)
                {
                    itemsN.AddRange(GetFileIf(dirItem.Items, filePredicate));
                }
                else if (item is FileItem fItem)
                {
                    if (filePredicate(fItem))
                    {
                        itemsN.Add(item);
                    }
                }
            }

            return itemsN;
        }
    }
}