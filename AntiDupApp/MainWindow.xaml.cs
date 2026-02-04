using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace AntiDupApp
{
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string? SelectedDirectory = dialog.FileName;

                if (Directory.Exists(SelectedDirectory))
                {
                    var duplicates = DuplicateFinder.GetDuplicateFilesByGroups(SelectedDirectory);

                    ((MainViewModel)DataContext).DisplayFiles(duplicates);
                }
            }
        }
    }

    public class WorkFile
    {
        public required string FileName { get; set; }
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<WorkFile> WorkFiles { get; set; }

        public MainViewModel()
        {
            WorkFiles = new ObservableCollection<WorkFile>();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        internal void DisplayFiles(IEnumerable<List<(DateTime, int, string)>> duplicateGroups)
        {
            WorkFiles.Clear();

            foreach (var group in duplicateGroups)
            {
                // Console.WriteLine("===========================================================================================");

                var duplicaGroup = group.OrderBy(t => t.Item1).ToList();
                foreach (var file in duplicaGroup)

                    // Console.WriteLine("  " + file.Item1.ToString("yyMMdd.HHmmss") + " - " + file.Item2 + " - " + file.Item3);

                    WorkFiles.Add(new WorkFile { FileName = file.Item3 });


            }
        }
    }
}