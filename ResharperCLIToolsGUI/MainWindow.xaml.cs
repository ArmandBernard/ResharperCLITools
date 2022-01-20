using Microsoft.Win32;
using System;
using System.Windows;

namespace ResharperCLIToolsGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "Solution File|*.sln",
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() != true) 
            {
                return;
            }
        }
    }
}
