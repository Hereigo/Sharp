using System;
using System.IO;

namespace A_TEST_48
{
    internal class PROGRAM
    {
        static readonly string DownloadsFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads\\";

        static void Main(string[] args)
        {

            string TEST_DIR = Path.Combine(DownloadsFolder, "EMAILS");

            Extract_Html_FromBase64_EML.ProcessEmlFilesInFolder(TEST_DIR);

            EmailFixResponsiveness.ProcessHtmlFilesInFolder(TEST_DIR);


            Console.WriteLine(" Done.");
        }
    }
}
