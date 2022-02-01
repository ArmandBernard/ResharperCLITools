﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using ResharperToolsLib;
using System.Linq;
using ResharperToolsLib.Logging;
using ResharperToolsLib.Tree;
using ResharperToolsLib.Config;
using ResharperCLIToolsGUI.Models;

namespace ResharperCLIToolsGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Solution? SavedSolution { get; set; }

        private ConfigLoader<ConfigModel> ConfigLoader { get; set; }

        private ConfigModel _Config;
        private ConfigModel Config 
        {
            get => _Config;
            set
            {
                _Config = value;
                ConfigLoader.SaveConfig(value);
            }
        }

        private ILogger Logger { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowContext();

            Logger = new Logger();

            ConfigLoader = new ConfigLoader<ConfigModel>(Logger);

            _Config = new ConfigModel(Array.Empty<Solution>());

            ConfigModel? config = ConfigLoader.ReadConfig(_Config);

            if (config == null)
            {
                Logger.Fatal("Config incorrectly formatted");
                Close();
                return;
            }

            Config = config;
            
            if (Config.RecentSolutions.Length > 0)
            {
                SavedSolution = Config.RecentSolutions[0];

                PopulateFileTree(SavedSolution.Path);
            }            
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = SavedSolution?.Path ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "Solution File|*.sln",
                ValidateNames = true
            };

            if (openFileDialog.ShowDialog() != true)
            {
                return;
            }

            var file = new FileInfo(openFileDialog.FileName);

            SavedSolution = new Solution(file.Directory!.FullName, DateTime.Now);

            if (Config.RecentSolutions == null)
            {
                Config = new ConfigModel(new [] { SavedSolution });
            }
            else
            {
                // add the saved solution to the start of the list, removing it anywhere else in
                // the list if it existed
                Config = new ConfigModel((new[] { SavedSolution }).Concat(Config.RecentSolutions.Where(s => s.Path != SavedSolution.Path)).ToArray());
            }

            PopulateFileTree(SavedSolution.Path);
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
            if (SavedSolution == null) { return; }

            var commandRunner = new CommandRunner(new Logger());

            commandRunner.EnsureToolsInstalled();

            var commandBuilder = new CommandBuilder(SavedSolution.Path);

            var treeItems = ((MainWindowContext) DataContext).TreeItems;

            var checkedPaths = GetFileIf(treeItems, (item) => item.IsChecked).Select(item => item.Path).ToArray();

            var command = commandBuilder.Clean(checkedPaths);

            commandRunner.Run(command);
        }

        private void CleanAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (SavedSolution == null) { return; }

            var commandRunner = new CommandRunner(new Logger());

            commandRunner.EnsureToolsInstalled();

            var commandBuilder = new CommandBuilder(SavedSolution.Path);

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
            var itemsN = new List<ITreeItem>();

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