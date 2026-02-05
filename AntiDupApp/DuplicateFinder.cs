using System.IO;
using System.Security.Cryptography;

namespace AntiDupApp;

internal static class DuplicateFinder
{
    internal static IEnumerable<List<(DateTime, int, string)>> GetDuplicateFilesByGroups(string rootDir)
    {
        var allFiles = Directory.EnumerateFiles(rootDir, "*", SearchOption.AllDirectories);

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
