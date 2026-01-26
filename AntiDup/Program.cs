using System.Security.Cryptography;
using System.Text;

class DuplicateFinder
{
    static void Main()
    {
        string userDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        var roots = new[]
        {
            Path.Combine(userDirectory, "aaa\\empty"),
            Path.Combine(userDirectory, "aaa\\tax")
        };

        var duplicates = FindDuplicateFiles(roots);

        Console.OutputEncoding = Encoding.UTF8;

        foreach (var group in duplicates)
        {
            Console.WriteLine("===========================================================================================");
            var duplicaGroup = group.OrderBy(t => t.Item1).ToList();
            foreach (var file in duplicaGroup)
                Console.WriteLine("  " + file.Item1.ToString("yyMMdd.HHmmss") + " - " + file.Item2 + " - " + file.Item3);
        }
    }

    static IEnumerable<List<(DateTime, int, string)>> FindDuplicateFiles(IEnumerable<string> rootDirs)
    {
        var allFiles = rootDirs
            .SelectMany(d => Directory.EnumerateFiles(d, "*", SearchOption.AllDirectories));

        var sizeGroups = allFiles
            .GroupBy(f => new FileInfo(f).Length)
            .Where(g => g.Count() > 1);

        var hashGroups = new Dictionary<string, List<(DateTime, int, string)>>();

        foreach (var group in sizeGroups)
        {
            foreach (var fName in group)
            {
                int fSize = (int)new FileInfo(fName).Length;

                DateTime fDateTime = File.GetLastWriteTimeUtc(fName);

                string hash = ComputeHash(fName);

                if (!hashGroups.ContainsKey(hash))
                    hashGroups[hash] = new List<(DateTime, int, string)>();

                hashGroups[hash].Add((fDateTime, fSize, fName));
            }
        }

        return hashGroups.Values.Where(g => g.Count > 1);
    }

    static string ComputeHash(string filePath)
    {
        using var stream = File.OpenRead(filePath);
        using var sha = SHA256.Create();
        return BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "");
    }
}
