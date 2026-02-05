using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace AntiDupApp;

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
