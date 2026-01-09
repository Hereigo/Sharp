namespace AAA_TEST_Console
{
    internal class PROGRAM
    {
        readonly string DownloadsFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads\\";

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
