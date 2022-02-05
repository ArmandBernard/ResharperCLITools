using Microsoft.Win32;
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
using System.Windows.Controls;

namespace ResharperCLIToolsGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Solution? CurrentSolution { get; set; }

        private ConfigLoader<ConfigModel> ConfigLoader { get; set; }

        private ConfigModel _Config;
        private ConfigModel Config 
        {
            get => _Config;
            set
            {
                _Config = value;
                ConfigLoader.SaveConfig(value);
                Recents.ItemsSource = _Config.RecentSolutions;
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
            
            // if there is at least one solution in the history
            if (Config.RecentSolutions.Length > 0)
            {
                var dirPath = Path.GetDirectoryName(Config.RecentSolutions[0].Path);

                // if the directory is found
                if (dirPath != null)
                {
                    // load it in
                    LoadSolution(Config.RecentSolutions[0]);
                }                
            }            
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = CurrentSolution?.Path ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "Solution File|*.sln",
                ValidateNames = true
            };

            if (openFileDialog.ShowDialog() != true)
            {
                return;
            }

            var file = new FileInfo(openFileDialog.FileName);

            var savedSolution = Config.RecentSolutions.FirstOrDefault(s => s.Path == file.FullName);

            LoadSolution(savedSolution ?? new Solution(Path.GetFileNameWithoutExtension(file.Name), file.FullName, DateTime.Now, new List<ITreeItem>()));
        }

        private void LoadSolution(Solution solution)
        {
            var dirPath = Path.GetDirectoryName(solution.Path);

            // if the directory is found
            if (dirPath != null)
            {
                // try to load in files
                var files = RefetchFiles(solution);

                // cancel if failed
                if (files == null)
                {
                    MessageBox.Show("Failed to load file list");
                }

                solution = new Solution(solution.Name, solution.Path, solution.LastOpened, files);

                // load it in
                CurrentSolution = solution;

                PopulateFileTree(solution.SelectedFiles);

                if (Config.RecentSolutions == null)
                {
                    Config = new ConfigModel(new[] { solution });
                }
                else
                {
                    // add the saved solution to the start of the list, removing it anywhere else in
                    // the list if it existed
                    Config = new ConfigModel((new[] { solution }).Concat(Config.RecentSolutions.Where(s => s.Path != solution.Path)).ToArray());
                }                
            }
        }

        private IList<ITreeItem>? RefetchFiles(Solution solution)
        {
            var dirPath = Path.GetDirectoryName(solution.Path);

            // if the directory is found
            if (dirPath == null)
            {
                return null;
            }

            IList<ITreeItem> scannedTreeItems;

            try
            {
                // scan file system for files
                scannedTreeItems = FileTree.GetTreeItems(dirPath);
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to load files", ex);

                return null;
            }

            // bring in saved check information
            return FileTree.ImportCheckState(scannedTreeItems, solution.SelectedFiles ?? Array.Empty<ITreeItem>());
        }

        private void PopulateFileTree(IList<ITreeItem> items)
        {
            ((MainWindowContext) DataContext).TreeItems = items;

            // shouldn't have to do this, but I can't get binding working
            treeView.ItemsSource = ((MainWindowContext) DataContext).TreeItems;
        }

        private void CleanButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentSolution == null) { return; }

            var commandRunner = new CommandRunner(new Logger());

            commandRunner.EnsureToolsInstalled();

            var commandBuilder = new CommandBuilder(CurrentSolution.Path);

            var treeItems = ((MainWindowContext) DataContext).TreeItems;

            var checkedPaths = GetFileIf(treeItems, (item) => item.IsChecked).Select(item => item.Path).ToArray();

            var command = commandBuilder.Clean(checkedPaths);

            commandRunner.Run(command);
        }

        private void CleanAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentSolution == null) { return; }

            var commandRunner = new CommandRunner(new Logger());

            commandRunner.EnsureToolsInstalled();

            var commandBuilder = new CommandBuilder(CurrentSolution.Path);

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

        private void RecentsItem_Click(object sender, RoutedEventArgs e)
        {
            var item = (MenuItem)sender;

            var solution = (Solution)item.DataContext;

            string? path = Path.GetDirectoryName(solution.Path);

            if(path == null)
            {
                Logger.Warn($"Solution not found at {solution.Path}");
                MessageBox.Show("Solution not found");
                return;
            }

            LoadSolution(solution);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            //LoadSolution(SavedSolution);

            ConfigLoader.SaveConfig(Config);

            base.OnClosing(e);
        }

        private void CheckBox_CheckChanged(object sender, RoutedEventArgs e)
        {
            //CurrentSolution = new Solution(CurrentSolution.Name, CurrentSolution.Path, CurrentSolution.LastOpened, ((MainWindowContext)DataContext).TreeItems);

            // add the saved solution to the start of the list, removing it anywhere else in
            // the list if it existed
            //Config = new ConfigModel((new[] { CurrentSolution }).Concat(Config.RecentSolutions.Where(s => s.Path != CurrentSolution.Path)).ToArray());
        }
    }
}