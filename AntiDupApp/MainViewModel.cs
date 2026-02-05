using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AntiDupApp;

public class MainViewModel : INotifyPropertyChanged
{
    public ObservableCollection<WorkFileModel> WorkFiles { get; set; }

    public MainViewModel()
    {
        WorkFiles = new ObservableCollection<WorkFileModel>();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    internal void DisplayFiles(IEnumerable<List<(DateTime, int, string)>> duplicateGroups)
    {
        WorkFiles.Clear();

        foreach (var group in duplicateGroups)
        {
            WorkFiles.Add(new WorkFileModel
            {
                // TODO: Consider a better way to represent group headers
                FileDate = null,
                FileSize = null,
                FileName = "==========================================================================================="
            });
            var duplicaGroup = group.OrderBy(t => t.Item1).ToList();
            foreach (var file in duplicaGroup)
            {
                WorkFiles.Add(new WorkFileModel
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
