namespace AAA_TEST_Console
{
    internal class PROGRAM
    {
        static readonly string DownloadsFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads\\";
        static readonly string UserProfileFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\";

        static void Main(string[] args)
        {
            // ShiftCharsInString.Test();

            // Delegates.TestDelegates();           

            // Delegate_min.Test();

            // var drives = DriveInfo.GetDrives();

            // ChromiumUpdate.GetApiData();

            // HtmlAgilityPack.Test();



            string TEST_DIR = Path.Combine(UserProfileFolder, "EMAILS");

            Extract_Html_FromBase64_EML.ProcessEmlFilesInFolder(TEST_DIR);

            EmailFixResponsiveness.ProcessHtmlFilesInFolder(TEST_DIR);




            Console.WriteLine(" Done.");
        }

    }
}
