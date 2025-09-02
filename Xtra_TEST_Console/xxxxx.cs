using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace AAA_TEST_Console
{
    public class AAAAA
    {
        public string Success { get; set; } // true,
        public DateTime CreationDate { get; set; } // "2025-08-29T16:54:59.8077314Z DATE !!!!!!!!!!!!!!
        public Guid SubmissionId { get; set; } // "4423
        public string Message { get; set; } // "rejected"
        public string NextAction { get; set; } // "None",
    }

    internal class xxxxx
    {
        static void Main(string[] args)
        {

            // Shift.Test();

            // Delegates.TestDelegates();           

            // Delegate_min.Test();

            // var drives = DriveInfo.GetDrives();

            // ChromiumUpdate.GetApiData();

            string TEST = "{\r\n  \"SubmissionId\": \"442329a5-83c4-4696-831b-312492982b81\",\r\n  \"Success\": true,\r\n  \"NextAction\": \"None\",\r\n  \"CreationDate\": \"2025-08-29T16:56:27.8077314Z\",\r\n  \"Message\": \"rejected\"\r\n}";

            try
            {
                AAAAA aaaa = JsonConvert.DeserializeObject<AAAAA>(TEST);
            }
            catch (Exception)
            {
                dynamic aaa = JsonConvert.DeserializeObject(TEST);

                var TSET2 = aaa.SubmissionId;
            }

            Guid submissionId = new();

            var BreakPoint = true;
        }

        static void Test234()
        {

            if (false && false || true)
            {
                Console.WriteLine("TRUE");
            }

            string htmlContent = @"
            <html>
                <body>
                    <h1>Welcome to My Website</h1>
                    <img src='old-image.jpg' alt='Old Image' />
                    <p>Some content...</p>
                    <img src='another-old-image.jpg' alt='Another Image' />
                </body>
<TABLE CELLSPACING=""0"" CELLPADDING=""0"" BORDER=""0"" style=""WIDTH:152.13mm;min-width: 152.13mm;HEIGHT:0.26mm;overflow:hidden;border:1pt none Black;background-color:Black;"">
<TR><TD style=""HEIGHT:0.26mm;WIDTH:152.13mm;min-width: 152.13mm;font-size:1pt"">&nbsp;</TD></TR></TABLE></TD></TR><TR><TD style=""HEIGHT:0.71mm;WIDTH:0.00mm""></TD><TD COLSPAN=""11"" style=""WIDTH:162.33mm;min-width: 162.33mm;HEIGHT:0.71mm;""></TD></TR>
<TR VALIGN=""top""><TD style=""HEIGHT:9.79mm;WIDTH:0.00mm""></TD><TD COLSPAN=""10"" style=""WIDTH:105.91mm;min-width: 105.91mm;HEIGHT:9.79mm;""></TD><TD ROWSPAN=""2"" style=""WIDTH:56.42mm;min-width: 56.42mm;""><DIV style=""WIDTH:46.22mm;"">
<DIV style=""min-width: 46.22mm;HEIGHT:15.00mm;WIDTH:46.22mm;overflow:hidden;padding-left:0pt;padding-top:0pt;padding-right:0pt;padding-bottom:0pt;border:1pt none Black;""><IMG onerror=""this.errored=true;"" SRC=""http://fake-address.org/images/some-fake-pic.png""/></DIV></DIV></TD></TR>
<TR VALIGN=""top""><TD style=""HEIGHT:5.21mm;WIDTH:0.00mm""></TD><TD style=""WIDTH:12.67mm;min-width: 12.67mm;HEIGHT:5.21mm;""></TD><TD ROWSPAN=""2"" COLSPAN=""10"" style=""WIDTH:101.78mm;min-width: 101.78mm;""><TABLE CELLSPACING=""0"" CELLPADDING=""0"" LANG=""en-US"" style="""">
<TR><TD style=""WIDTH:101.78mm;min-width: 101.78mm;HEIGHT:6.00mm;word-wrap:break-word;white-space:pre-wrap;padding-left:2pt;padding-top:2pt;padding-right:2pt;padding-bottom:2pt;border:1pt none Black;background-color:Transparent;font-style:normal;font-family:Arial;font-size:11pt;font-weight:700;text-decoration:none;unicode-bidi:normal;color:#000000;direction:ltr;layout-flow:horizontal;writing-mode:lr-tb;vertical-align:top;text-align:left;"">The Direct Debit Guarantee</TD></TR>
</TABLE></TD><TD style=""WIDTH:4.13mm;min-width: 4.13mm;HEIGHT:5.21mm;""></TD></TR><TR><TD style=""HEIGHT:0.79mm;WIDTH:0.00mm""></TD><TD style=""WIDTH:12.67mm;min-width: 12.67mm;HEIGHT:0.79mm;""></TD><TD ROWSPAN=""2"" COLSPAN=""2"" style=""WIDTH:60.56mm;min-width: 60.56mm;HEIGHT:0.79mm;""></TD></TR><TR><TD style=""HEIGHT:1.76mm;WIDTH:0.00mm""></TD><TD COLSPAN=""11"" style=""WIDTH:114.44mm;min-width: 114.44mm;HEIGHT:1.76mm;""></TD></TR><TR VALIGN=""top"">
<TD style=""HEIGHT:6.00mm;WIDTH:0.00mm""></TD><TD ROWSPAN=""2"" COLSPAN=""2"" style=""WIDTH:12.67mm;min-width: 12.67mm;HEIGHT:6.00mm;""></TD><TD COLSPAN=""11"" style=""WIDTH:162.33mm;min-width: 162.33mm;"">
<TABLE CELLSPACING=""0"" CELLPADDING=""0"" LANG=""en-US"" style="""">
<TR><TD style=""WIDTH:152.13mm;min-width: 152.13mm;HEIGHT:6.00mm;word-wrap:break-word;white-space:pre-wrap;padding-left:2pt;padding-top:2pt;padding-right:2pt;padding-bottom:2pt;border:1pt none Black;background-color:Transparent;font-style:normal;font-family:Arial;font-size:11pt;font-weight:400;text-decoration:none;unicode-bidi:normal;color:#000000;direction:ltr;layout-flow:horizontal;writing-mode:lr-tb;vertical-align:top;text-align:left;"">This Guarantee is offered by all banks and building societies that accept instructions to pay Direct Debits.</TD></TR></TABLE>
                <body>
                    <img src=""image1.jpg"" alt=""image1"">
                    <img src=""image2.jpg"" alt=""image2"" width=""500"" height=""300"">
                </body>
            </html>";

            string pattern = @"<img\s+([^>]+?)\s*src\s*=\s*['""][^'""]+['""]([^>]*?)>";

            string replacedHtml = Regex.Replace(htmlContent, pattern, match =>
            {
                string updatedSrc = "LOCAL-LOGO.PNG";

                string imgTag = match.Value;

                if (imgTag.Contains("fake-address.org"))
                {
                    string newImgTag = Regex.Replace(imgTag, @"\s*src\s*=\s*['""][^'""]+['""]", $" src=\"{updatedSrc}\"", RegexOptions.IgnoreCase);

                    return newImgTag;
                }
                return imgTag;
            },
            RegexOptions.IgnoreCase);

            Console.WriteLine(replacedHtml);


            Console.WriteLine("\r\n Done.");
        }
    }
}
