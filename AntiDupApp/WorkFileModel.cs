using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.VisualBasic.FileIO;

namespace AntiDupApp;

public class WorkFileModel : INotifyPropertyChanged
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

    public WorkFileModel()
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
