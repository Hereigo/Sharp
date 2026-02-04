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
            if (((ListBox)sender).SelectedItem == null)
                return;

            var path1 = ((WorkFile)((ListBox)sender).SelectedItem).FileName;

            if (((WorkFile)((ListBox)sender).SelectedItem).FileName is string filePath)
            {
                if (!File.Exists(filePath))
                    return;

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
        public DateTime FileDate { get; internal set; }
        public string FileDateString => FileDate.ToString("yyMMdd.HHmmss");
        public int FileSize { get; internal set; }
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
                {
                    WorkFiles.Add(new WorkFile
                    {
                        FileDate = file.Item1,
                        FileSize = file.Item2,
                        FileName = file.Item3
                    });
                }

            }
        }
    }
}