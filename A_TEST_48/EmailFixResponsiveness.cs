using System;
using System.IO;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace A_TEST_48
{
    /// <summary>
    /// Minimally modifies SSRS-generated email HTML to make it responsive on mobile email clients
    /// (e.g., Outlook iOS/Android, Apple Mail, Gmail app).
    /// Pure inline-style approach — no embedded &lt;style&gt; block, works on any screen width.
    /// </summary>
    public class EmailFixResponsiveness
    {
        internal static void ProcessHtmlFilesInFolder(string filesFolder)
        {
            foreach (var file in Directory.GetFiles(filesFolder, "*.html"))
            {
                Console.WriteLine($"Processing file: {file}");
                var outputFile = Path.Combine(filesFolder, Path.GetFileNameWithoutExtension(file) + "_fixed.html");
                if (File.Exists(outputFile)) File.Delete(outputFile);
                var htmlContent = File.ReadAllText(file);
                var fixer = new EmailFixResponsiveness();
                var fixedHtml = fixer.MakeEmailResponsive(htmlContent);
                File.WriteAllText(outputFile, fixedHtml);
            }
        }

        // Force styles for the standard safe email width ~184–200mm (600px).
        private const string ResponsiveStyles = @"
        body { margin: 0; padding: 0; }
        #oReportDiv { max-width: 800px; margin: 0 auto; }
        #oReportDiv img { max-width: 100%; height: auto; }
        
        @media only screen and (max-width: 600px) {
            html, body { margin: 0 !important; padding: 0 !important; width: 100% !important; }
            #oReportDiv,
            #oReportDiv * {
                max-width: 100% !important;
                min-width: 0 !important;
                box-sizing: border-box;
            }
            #oReportDiv,
            #oReportDiv div { overflow: visible !important; overflow-x: visible !important; width: auto !important; height: auto !important; }
            #oReportDiv table { width: 100% !important; table-layout: auto !important; }
            #oReportDiv td, #oReportDiv th { width: auto !important; height: auto !important; word-break: break-word; }
            #oReportDiv span, #oReportDiv p, #oReportDiv div { white-space: normal !important; }
        }
        ";


        // Matches an inline 'min-width: <value>;' declaration (terminated by ; or end of style).
        private static readonly Regex MinWidthDeclarationRegex = new Regex(
            @"min-width\s*:\s*[^;""']+\s*;?",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Matches 'overflow: auto;' (any whitespace).
        private static readonly Regex OverflowAutoRegex = new Regex(
            @"overflow\s*:\s*auto\s*;?",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Matches 'overflow-x: hidden;' / 'overflow-y: hidden;'.
        private static readonly Regex OverflowAxisHiddenRegex = new Regex(
            @"overflow-(?:x|y)\s*:\s*hidden\s*;?",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public string MakeEmailResponsive(string emailHtmlBodyContent)
        {
            if (string.IsNullOrWhiteSpace(emailHtmlBodyContent))
            {
                return emailHtmlBodyContent;
            }

            var doc = new HtmlDocument
            {
                OptionWriteEmptyNodes = true,
                OptionAutoCloseOnEnd = true,
                OptionDefaultStreamEncoding = System.Text.Encoding.UTF8
            };
            doc.LoadHtml(emailHtmlBodyContent);

            EnsureViewportAndStyle(doc);
            StripInlineMinWidth(doc);
            RelaxReportContainerOverflow(doc);

            return doc.DocumentNode.OuterHtml;
        }

        private static void EnsureViewportAndStyle(HtmlDocument doc)
        {
            var head = doc.DocumentNode.SelectSingleNode("//head");
            if (head == null)
            {
                head = doc.CreateElement("head");
                var html = doc.DocumentNode.SelectSingleNode("//html");
                if (html != null)
                {
                    html.PrependChild(head);
                }
                else
                {
                    doc.DocumentNode.PrependChild(head);
                }
            }

            if (!HasViewportMeta(head))
            {
                var viewport = doc.CreateElement("meta");
                viewport.SetAttributeValue("name", "viewport");
                viewport.SetAttributeValue("content", "width=device-width, initial-scale=1.0");
                head.AppendChild(viewport);
            }

            var styleNode = doc.CreateElement("style");
            styleNode.SetAttributeValue("type", "text/css");
            styleNode.AppendChild(doc.CreateTextNode(ResponsiveStyles));
            head.AppendChild(styleNode);
        }

        private static bool HasViewportMeta(HtmlNode head)
        {
            foreach (var child in head.ChildNodes)
            {
                if (child.NodeType != HtmlNodeType.Element)
                    continue;

                if (!string.Equals(child.Name, "meta", StringComparison.OrdinalIgnoreCase))
                    continue;

                var name = child.GetAttributeValue("name", string.Empty);
                if (string.Equals(name, "viewport", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        private static void StripInlineMinWidth(HtmlDocument doc)
        {
            var styledNodes = doc.DocumentNode.SelectNodes("//*[@style]");
            if (styledNodes == null)
                return;

            foreach (var node in styledNodes)
            {
                var style = node.GetAttributeValue("style", string.Empty);
                if (string.IsNullOrEmpty(style))
                    continue;

                var updated = MinWidthDeclarationRegex.Replace(style, string.Empty);
                if (!string.Equals(updated, style, StringComparison.Ordinal))
                {
                    node.SetAttributeValue("style", updated.Trim());
                }
            }
        }

        private static void RelaxReportContainerOverflow(HtmlDocument doc)
        {
            var roots = doc.DocumentNode.SelectNodes("//*[@id='oReportDiv']");
            if (roots == null)
                return;

            foreach (var node in roots)
            {
                var style = node.GetAttributeValue("style", string.Empty);
                if (string.IsNullOrEmpty(style))
                    continue;

                var updated = OverflowAutoRegex.Replace(style, "overflow:visible;");
                updated = OverflowAxisHiddenRegex.Replace(updated, string.Empty);
                if (!string.Equals(updated, style, StringComparison.Ordinal))
                {
                    node.SetAttributeValue("style", updated.Trim());
                }
            }
        }
    }
}
