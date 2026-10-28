using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.VisualBasic.FileIO;

namespace AntiDupApp;

public class WorkFileModel : INotifyPropertyChanged
{
    public required string FileName { get; set; }
    
    public required string FileShortName { get; set; }

    public DateTime? FileDate { get; internal set; }
    public int? FileSize { get; internal set; }
    public string? FileDateString => FileDate?.ToString("yyMMdd.HHmmss");

    private bool _isButtonVisible;
    private bool _isTextDimmed;

    public bool IsTextDimmed
    {
        get => _isTextDimmed;
        set
        {
            _isTextDimmed = value;
            OnPropertyChanged();
        }
    }

    public bool IsButtonVisible
    {
        get => _isButtonVisible;
        set
        {
            _isButtonVisible = value;
            OnPropertyChanged();
        }
    }

    public ICommand ListItemButtonCommand { get; }

    public ICommand OpenDirCommand { get; }

    public WorkFileModel()
    {
        ListItemButtonCommand = new RelayCommand(OnAction);
        OpenDirCommand = new RelayCommand(OnOpenDir);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string prop = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

    private void OnOpenDir()
    {
        if (!File.Exists(FileName))
            return;

        // Opens the containing folder in Explorer with the file selected
        Process.Start(new ProcessStartInfo
        {
            FileName = "explorer.exe",
            Arguments = $"/select,\"{FileName}\"",
            UseShellExecute = true
        });
    }

    private void OnAction()
    {
        IsButtonVisible = false;
        IsTextDimmed = true;
        FileSystem.DeleteFile(
            FileName,
            UIOption.OnlyErrorDialogs,
            RecycleOption.SendToRecycleBin,
            UICancelOption.DoNothing
        );
    }
}
