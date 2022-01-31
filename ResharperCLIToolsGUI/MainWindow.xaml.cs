using Microsoft.Win32;
using ResharperCLIToolsGUI.Tree;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

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

            ((MainWindowContext)DataContext).TreeItems = items;

            // shouldn't have to do this, but I can't get binding working
            treeView.ItemsSource = ((MainWindowContext)DataContext).TreeItems;
        }
    }
}
