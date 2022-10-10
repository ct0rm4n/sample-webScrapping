using System;
using WebScraping.Services;
namespace WebScraping
{
    public class Program
    {
        static WebScrapingService _webScrapingService = new WebScrapingService();
        static void Main(string[] args)
        {
            Console.WriteLine("Scraping for you DB!");
            var products = _webScrapingService.GetAllProductByVendor("_CustId_45931133").Result;
            if (products?.Count > 0)
            {
                Console.WriteLine("Great work, its fine");
                Console.WriteLine("Total of products {0}",products.Count);
                foreach(var p in products)
                {
                    Console.WriteLine($"Link: {p}");
                    var product = _webScrapingService.GetProductByUrl(p).Result;
                }
            }
            else
            {
                Console.WriteLine("Has a problem");
            }
            Console.ReadLine();
        }
    }
}
