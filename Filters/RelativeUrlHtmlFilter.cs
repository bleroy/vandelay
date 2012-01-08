using System;
using System.IO;
using System.Text;
using HtmlAgilityPack;
using Orchard;
using Orchard.Environment.Extensions;
using Orchard.Services;

namespace Vandelay.Industries.Filters {
    [OrchardFeature("Vandelay.RelativeUrlFilter")]
    public class RelativeUrlHtmlFilter : IHtmlFilter {
        private readonly IWorkContextAccessor _wca;

        public RelativeUrlHtmlFilter(IWorkContextAccessor wca) {
            _wca = wca;
        }

        public string ProcessContent(string text, string flavor) {
            var wc = _wca.GetContext();
            var baseUrl = wc.CurrentSite.BaseUrl;
            if (string.IsNullOrWhiteSpace(baseUrl)) return text;
            var baseUri = new Uri(baseUrl);
            var doc = new HtmlDocument();
            doc.LoadHtml(text);
            var links = doc.DocumentNode.SelectNodes("//a[@href]");
            if (links != null) {
                foreach (var link in links) {
                    MakeAbsolute(link.Attributes["href"], baseUri);
                }
            }
            var imgs = doc.DocumentNode.SelectNodes("//img[@src]");
            if (imgs != null) {
                foreach (var img in imgs) {
                    MakeAbsolute(img.Attributes["src"], baseUri);
                }
            }
            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb)) {
                doc.Save(writer);
                return sb.ToString();
            }
        }

        private static void MakeAbsolute(HtmlAttribute attr, Uri baseUrl) {
            var url = attr.Value;
            if (!url.StartsWith("http:", StringComparison.OrdinalIgnoreCase)
                && !url.StartsWith("https:", StringComparison.OrdinalIgnoreCase)
                && Uri.IsWellFormedUriString(url, UriKind.Relative)) {
                attr.Value = new Uri(baseUrl, url).ToString();
            }
        }
    }
}