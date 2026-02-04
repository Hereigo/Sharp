using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace AntiDupApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var path1 = ((WorkFile)((ListBox)sender).SelectedItem).FileName;

            if (((WorkFile)((ListBox)sender).SelectedItem).FileName is string filePath)
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
            }
        }

        public ICommand BrowseDirectoryCommand => new RelayCommand(() =>
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string? SelectedDirectory = dialog.FileName;

                LoadFiles(SelectedDirectory);
            }
        });

        private void LoadFiles(string selectedDirectory)
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = "Select a folder"
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string selectedPath = dialog.FileName;

                var files = Directory.GetFiles(selectedPath);

                // FilePaths.Clear();
                // 
                // foreach (var file in files)
                //     FilePaths.Add(file);
            }
        }
    }

    public class WorkFile
    {
        public required string FileName { get; set; }
    }

    public class MainViewModel
    {
        public ObservableCollection<WorkFile> WorkFiles { get; set; }

        public MainViewModel()
        {
            WorkFiles = new ObservableCollection<WorkFile>
            {
                new WorkFile { FileName = @"C:\temp\2652.pdf" },
                new WorkFile { FileName = @"C:\temp\2652.xml" }
            };
        }
    }
}