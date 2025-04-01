using System.Collections.Concurrent;

namespace Calendarium.Utils
{
    public interface IContentManager
    {
        string GetVersionedContentFileLink(string wwwrootFile);
    }

    public class ContentManager : IContentManager
    {
        private ConcurrentDictionary<string, string> _cachedVersions = new ConcurrentDictionary<string, string>();
        private object _cachedVersionsLocker = new object();

        public string GetVersionedContentFileLink(string wwwrootFile)
        {
            string result;
            if (!_cachedVersions.TryGetValue(wwwrootFile, out result))
            {
                lock (_cachedVersionsLocker)
                {
                    if (!_cachedVersions.TryGetValue(wwwrootFile, out result))
                    {
                        result = CalcFileMD5Hash(Environment.CurrentDirectory + "\\wwwroot" + wwwrootFile);
                        _cachedVersions.TryAdd(wwwrootFile, result);
                    }
                }
            }
            return $"{wwwrootFile}?v={result}";
        }

        public static string CalcFileMD5Hash(string fullFilePath)
        {
            string fileHash = string.Empty;

            if (File.Exists(fullFilePath))
            {
                using (var md5 = System.Security.Cryptography.MD5.Create())
                {
                    using (var stream = File.OpenRead(fullFilePath))
                    {
                        fileHash = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
                    }
                }
            }
            return fileHash;
        }
    }
}
