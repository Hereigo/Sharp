using System.IO;

namespace AAA_TEST_Console
{
    internal class PROGRAM
    {
        static readonly string DownloadsFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads\\";
        static readonly string UserProfileFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\";

        static void Main(string[] args)
        {
            ShiftCharsInString.Test();

            // Delegates.TestDelegates();           

            // Delegate_min.Test();

            // var drives = DriveInfo.GetDrives();

            // ChromiumUpdate.GetApiData();

            Console.WriteLine("\r\n Done.");
        }

    }
}
