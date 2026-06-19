using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        // TEST !!!!!!!!
        // TEST !!!!!!!!
        internal static void ProcessHtmlFilesInFolder(string filesFolder)
        {
            foreach (var file in Directory.GetFiles(filesFolder, "*.html"))
            {
                Console.WriteLine($"Processing file: {file}");
                var outputFile = Path.Combine(filesFolder, Path.GetFileNameWithoutExtension(file) + "_fixed.html");
                if (File.Exists(outputFile)) File.Delete(outputFile);
                var htmlContent = File.ReadAllText(file);
                var fixer = new EmailFixResponsiveness();
                var fixedHtml = fixer.MakeResponsive(htmlContent);
                File.WriteAllText(outputFile, fixedHtml);
            }
        }
        // TEST !!!!!!!!
        // TEST !!!!!!!!


        // private readonly ILogger _requestLogger;
        // 
        // public EmailFixResponsiveness(ILogger requestLogger)
        // {
        //     _requestLogger = requestLogger;
        // }

        // Force styles for the standard safe email width ~184–200mm (600px).
        private const string ResponsiveStyles = @"
            * { box-sizing: border-box; }
            body { margin: 0; padding: 0; background-color: #ffffff; }
            .ssrs-email {
                max-width: 600px;
                width: 100%;
                margin: 0 auto;
                padding: 20px 16px;
                direction: ltr;
            }
            .ssrs-email div { max-width: 100%; }
            @media screen and (max-width: 600px) {
                .ssrs-email { padding: 12px 8px; }
                span { white-space: normal !important; }
            }
        ";

        public string MakeResponsive(string html)
        {
            // _requestLogger.Write(LogLevel.Debug, LogKind.Etc, "[RenderEmailBody.MakeResponsive] Starting ...");

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            PrepareHead(doc);

            // Essensial SSRS reports element.
            var reportDiv = doc.GetElementbyId("oReportDiv");
            if (reportDiv == null)
                return doc.DocumentNode.OuterHtml;

            var contentNodes = ExtractContent(doc, reportDiv);

            // _requestLogger.Write(LogLevel.Debug, LogKind.Etc, $"[RenderEmailBody.MakeResponsive] {contentNodes.Count} Content Nodes will be processed.");

            reportDiv.RemoveAllChildren();
            reportDiv.Attributes.Remove("style");
            reportDiv.Attributes.Remove("dir");

            var emailBodyContainer = doc.CreateElement("div");
            emailBodyContainer.SetAttributeValue("class", "ssrs-email");

            foreach (var node in contentNodes)
                emailBodyContainer.AppendChild(node);

            reportDiv.AppendChild(emailBodyContainer);

            // _requestLogger.Write(LogLevel.Debug, LogKind.Etc, $"[RenderEmailBody.MakeResponsive] Successfully finished.");

            return doc.DocumentNode.OuterHtml;
        }

        private void PrepareHead(HtmlDocument doc)
        {
            var head = doc.DocumentNode.SelectSingleNode("//head");
            if (head == null)
                return;

            if (head.SelectSingleNode("meta[@name='viewport']") == null)
            {
                var viewport = doc.CreateElement("meta");
                viewport.SetAttributeValue("name", "viewport");
                viewport.SetAttributeValue("content", "width=device-width, initial-scale=1.0");
                head.PrependChild(viewport);
            }

            var style = doc.CreateElement("style");
            style.InnerHtml = ResponsiveStyles;
            head.AppendChild(style);

            // _requestLogger.Write(LogLevel.Debug, LogKind.Etc, "[RenderEmailBody.MakeResponsive] Email body header styled.");
        }

        private List<HtmlNode> ExtractContent(HtmlDocument doc, HtmlNode reportDiv)
        {
            // SSRS nests all email text 5 levels deep inside layout-only tables by default.
            // The innermost content container is always a DIV with overflow-x:hidden,
            // which holds the actual paragraph DIVs with text, spans, and links.
            // Hyperlink cells skip the overflow-x:hidden wrapper and place <a> directly in the TD.
            var contentContainers = reportDiv.SelectNodes(
                ".//div[contains(@style,'overflow-x:hidden')] | .//td[a]");

            if (contentContainers == null)
                return new List<HtmlNode> { reportDiv.CloneNode(true) };

            var resultHtmlNodes = new List<HtmlNode>();

            foreach (var container in contentContainers)
            {
                foreach (var child in container.ChildNodes.ToList())
                {
                    if (child.NodeType == HtmlNodeType.Text)
                        continue;

                    resultHtmlNodes.Add(ConvertNode(doc, child));
                }
            }

            return resultHtmlNodes;
        }

        private HtmlNode ConvertNode(HtmlDocument doc, HtmlNode node)
        {
            var style = node.GetAttributeValue("style", "");

            // Handle SSRS rendered visual line separators as ~0.26mm-tall divs with overflow:hidden.
            if (IsThinSpacer(style))
            {
                var spacer = doc.CreateElement("div");
                spacer.SetAttributeValue("style", "height:1px;margin:4px 0;");
                return spacer;
            }

            var div = node.CloneNode(true);
            var cleanStyle = StripFixedDimensions(style);

            if (string.IsNullOrWhiteSpace(cleanStyle))
                div.Attributes.Remove("style");
            else
                div.SetAttributeValue("style", cleanStyle);

            return div;
        }

        private bool IsThinSpacer(string style)
        {
            var match = Regex.Match(style, @"HEIGHT\s*:\s*([\d.]+)\s*mm", RegexOptions.IgnoreCase);
            return match.Success
                && double.TryParse(
                    match.Groups[1].Value,
                    System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out var height)
                && height < 2.0;
        }

        private string StripFixedDimensions(string style)
        {
            // Strip mm-based widths/heights that SSRS emits for print layout —
            // these prevent fluid reflow on narrow mobile screens.
            style = Regex.Replace(style, @"(?:min-width|WIDTH)\s*:\s*[\d.]+mm\s*;?\s*", "", RegexOptions.IgnoreCase);
            style = Regex.Replace(style, @"overflow-x\s*:\s*hidden\s*;?\s*", "", RegexOptions.IgnoreCase);
            style = Regex.Replace(style, @"HEIGHT\s*:\s*[\d.]+mm\s*;?\s*", "", RegexOptions.IgnoreCase);
            return style.Trim().TrimEnd(';');
        }
    }
}