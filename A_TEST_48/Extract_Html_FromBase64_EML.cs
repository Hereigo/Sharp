using System;
using System.IO;
using System.Linq;

namespace A_TEST_48
{
    internal class Extract_Html_FromBase64_EML
    {
        internal static void ProcessEmlFilesInFolder(string filesFolder)
        {
            foreach (var file in Directory.GetFiles(filesFolder, "*.eml"))
            {
                Console.WriteLine($"Processing file: {file}");

                string line;
                string base64string = string.Empty;
                string outputFilePath = Path.Combine(filesFolder, Path.GetFileNameWithoutExtension(file) + ".html");

                using (var reader = new StreamReader(file))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        // Skip empty lines and lines that contain mail headers (which usually contain a colon)
                        if (string.IsNullOrWhiteSpace(line) || line.Contains(':'))
                            continue;

                        base64string += line;
                    }
                }

                var bytes = Convert.FromBase64String(base64string);

                using (var imageFile = new FileStream(outputFilePath, FileMode.Create))
                {
                    imageFile.Write(bytes, 0, bytes.Length);
                    imageFile.Flush();
                }

                Console.WriteLine($"Saved processed content to: {outputFilePath}");
            }
        }
    }
}
