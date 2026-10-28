using System.Collections.ObjectModel;

namespace AntiDupApp;

public class MainViewModel
{
    public ObservableCollection<WorkFileModel> WorkFiles { get; set; }

    public MainViewModel()
    {
        WorkFiles = new ObservableCollection<WorkFileModel>();
    }

    internal void DisplayFiles(IEnumerable<List<(DateTime, int, string)>> duplicateGroups, string selectedDirectory)
    {
        WorkFiles.Clear();

        foreach (var group in duplicateGroups)
        {
            WorkFiles.Add(new WorkFileModel
            {
                // TODO: Consider a better way to represent group headers
                FileDate = null,
                //FileSize = null,
                FileShortName = "===========================================================================================",
                FileName = ""
            });
            var duplicaGroup = group.OrderBy(t => t.Item1).ToList();
            foreach (var file in duplicaGroup)
            {
                WorkFiles.Add(new WorkFileModel
                {
                    IsButtonVisible = true,
                    IsTextDimmed = false,
                    FileDate = file.Item1,
                    //FileSize = file.Item2,
                    FileShortName = file.Item3.ToString().Replace(selectedDirectory, "").TrimStart('\\'),
                    FileName = file.Item3
                });
            }
        }
    }
}
