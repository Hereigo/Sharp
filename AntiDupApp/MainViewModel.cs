using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.VisualBasic.FileIO;

namespace AntiDupApp;

public class WorkFile : INotifyPropertyChanged
{
    public required string FileName { get; set; }
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

    public WorkFile()
    {
        ListItemButtonCommand = new RelayCommand(OnAction);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string prop = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

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
            WorkFiles.Add(new WorkFile
            {
                // TODO: Consider a better way to represent group headers
                FileDate = null,
                FileSize = null,
                FileName = "==========================================================================================="
            });
            var duplicaGroup = group.OrderBy(t => t.Item1).ToList();
            foreach (var file in duplicaGroup)
            {
                WorkFiles.Add(new WorkFile
                {
                    IsButtonVisible = true,
                    IsTextDimmed = false,
                    FileDate = file.Item1,
                    FileSize = file.Item2,
                    FileName = file.Item3
                });
            }
        }
    }
}
