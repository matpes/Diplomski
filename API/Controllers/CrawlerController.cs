namespace API.Controllers
{
    using HtmlAgilityPack;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using System.Net;
    using System.Text;
    using System.IO;
    using Microsoft.AspNetCore.Mvc;
    using API.Data;
    using API.DTOs;
    using System.Linq;
    using System.Collections.Generic;
    using System;

    public class CrawlerController : BaseApiController
    {
        private readonly DataContext _context;
        public CrawlerController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("bershka")]
        public Task<ActionResult> crawlBershka(LoginDto loginDto)
        {
            var url = "https://www.bershka.com/rs/mu%C5%A1karci-c1010193133.html";
            var response = CallUrl(url).Result;

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response);

            var links = htmlDoc.DocumentNode.Descendants("li").Where(node => node.GetAttributeValue("class", "").Contains("category-line")).ToList();
            var selecting = false;
            List<string> narrowedLinks = new List<string>();
            var i = 0;
            while (i < links.Count)
            {
                if (!selecting)
                {
                    if (links.ElementAt<HtmlNode>(i).InnerText == "\n      Najprodavaniji proizvodi\n     ")
                    {
                        selecting = true;
                    }
                }
                else
                {
                    if (links.ElementAt<HtmlNode>(i).InnerText == "")
                    {
                        selecting = false;
                    }
                    else
                    {
                        //Dosli smo do proizvoda koje zelimo da crawlujemo
                        narrowedLinks.Add("https://www.bershka.com" + links.ElementAt<HtmlNode>(i).FirstChild.Attributes[0].Value);
                        //Console.WriteLine(links.ElementAt<HtmlNode>(i).InnerText);
                    }
                }
                i++;
            }

            //Imamo spremne sve linkove (svih 20)
            foreach (var newUrl in narrowedLinks)
            {
                response = CallUrl(newUrl).Result;

                htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response);


            }

            return null;
        }

        // [HttpGet("bershka")]
        // public async Task<ActionResult<UserDto>> Login()
        // {

        //     return Unauthorized("Inavlid si ti");


        // }

        private static async Task<string> CallUrl(string fullUrl)
        {
            HttpClient client = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
            client.DefaultRequestHeaders.Accept.Clear();
            var response = client.GetStringAsync(fullUrl);
            return await response;
        }

    }
}