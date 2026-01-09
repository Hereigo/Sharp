using System.Text.RegularExpressions;

namespace AAA_TEST_Console
{
    internal class HtmlPage_img_Replace
    {
        // Example usage:
        // string updatedHtml = ReplaceImgSrc(htmlContent, "fake-address.org", "LOCAL-LOGO.PNG");

        public string ReplaceImgSrc(string htmlContent, string targetDomain, string newImagePath)
        {
            string pattern = @"<img\s+([^>]+?)\s*src\s*=\s*['""][^'""]+['""]([^>]*?)>";
            string replacedHtml = Regex.Replace(htmlContent, pattern, match =>
            {
                string imgTag = match.Value;
                if (imgTag.Contains(targetDomain))
                {
                    string newImgTag = Regex.Replace(imgTag, @"\s*src\s*=\s*['""][^'""]+['""]", $" src=\"{newImagePath}\"", RegexOptions.IgnoreCase);
                    return newImgTag;
                }
                return imgTag;
            },
            RegexOptions.IgnoreCase);
            return replacedHtml;
        }
    }
}
