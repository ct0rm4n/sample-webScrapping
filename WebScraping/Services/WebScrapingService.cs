using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebScraping.Services
{
    public class WebScrapingService
    {
        public WebScrapingService()
        {
        }

        public Task<IList<string>> GetAllProductByVendor(string Id)
        {
            var wc = new WebClient();
            var page = wc.DownloadString($"https://lista.mercadolivre.com.br/{Id}");
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(page);
            IList<string> json = new List<string>();
            IList<string> classes = new List<string>();
            classes.Add("ui-search-layout__item");

            foreach (HtmlNode node in htmlDoc.DocumentNode.SelectNodes("//ol/li"))
            {
                if (node.Attributes.Count > 0)
                {
                    json.Add(node.Descendants("a")
                  .First(x => x.Attributes["class"] != null
                           && x.Attributes["class"].Value == "ui-search-link").Attributes["href"].Value);
                }
            }
            return Task.FromResult(json);
        }

        public Task<IList<Object>> GetProductByUrl(string url)
        {
            var wc = new WebClient();
            var page = wc.DownloadString($"{url}");
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(page);
            IList<Object> json = new List<Object>();
            IList<string> classes = new List<string>();
            classes.Add("ui-search-layout__item");
            
            json.Add(new
            {
                Name = htmlDoc.GetElementbyId("header").Descendants("h1")
                  .First(x => x.Attributes["class"] != null
                           && x.Attributes["class"].Value == "ui-pdp-title").InnerText,
                Price = htmlDoc.DocumentNode.SelectNodes("//span").Descendants()
                  .First(x => x.Attributes["class"] != null
                  && x.Attributes["class"].Value == "andes-money-amount__fraction").InnerText+
                  ","+
                    htmlDoc.DocumentNode.SelectNodes("//span").Descendants()
                      .First(x => x.Attributes["class"] != null
                      && x.Attributes["class"].Value.Contains("andes-money-amount__cents")).InnerHtml,
                Description = htmlDoc.DocumentNode.SelectNodes("//div").Descendants()
                  .First(x => x.Attributes["class"] != null
                  && x.Attributes["class"].Value.Contains("ui-pdp-description__content")).InnerText,
                StockQtd = BuildStockQuantity(htmlDoc)
            });
            
            return Task.FromResult(json);
        }
        public static string BuildStockQuantity(HtmlDocument htmlDoc)
        {
            var stock = "";
            try
            {
                stock =  htmlDoc.DocumentNode.SelectNodes("//span").Descendants()
                      .First(x => x.Attributes["class"] != null
                      && x.Attributes["class"].Value.Contains("ui-pdp-buybox__quantity__available")).InnerHtml;
                return stock;
            }
            catch (Exception)
            {
                stock = "(1)";
                return stock;
            }
        }
        public static IEnumerable<HtmlNode> GetElementsWithClass(HtmlDocument doc, String[] classNames)
        {

            Regex[] exprs = new Regex[classNames.Length];
            for (Int32 i = 0; i < exprs.Length; i++)
            {
                exprs[i] = new Regex("\\b" + Regex.Escape(classNames[i]) + "\\b", RegexOptions.Compiled);
            }

            return doc
                .DocumentNode.Descendants()
                //.Where(n => n.NodeType == NodeType.Element)
                .Where(e =>
                   e.Name == "div" &&
                   exprs.All(r =>
                      r.IsMatch(e.GetAttributeValue("class", ""))
                   )
                );
        }

    }
}
