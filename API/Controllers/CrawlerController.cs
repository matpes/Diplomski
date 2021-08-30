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

    using PuppeteerSharp;

    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;

    public class CrawlerController : BaseApiController
    {
        private readonly DataContext _context;


        public CrawlerController(DataContext context)
        {
            _context = context;
        }



        [HttpPost("bershka")]
        public ActionResult crawlBershka(LoginDto loginDto)
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
            //DOVDE SVE RADI


            // var options = new ChromeOptions()
            // {
            //     BinaryLocation = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe"
            // };
            // options.AddArguments(new List<string>() { "headless", "disable-gpu" });
            // var browser = new ChromeDriver(options);
            foreach (var newUrl in narrowedLinks)
            {
                response = CallUrl(newUrl).Result;

                htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response);

                var linkovi = htmlDoc.DocumentNode.Descendants("a").ToList();

                return Ok(response);

            }

            return null;
        }

        [HttpPost("zaraDetalj")]
        public ActionResult crawZaraDetails(LoginDto loginDto)
        {

            var url = "https://www.pullandbear.com/rs/muskarci/odeca/farmerke-n6347";
            var response = CallUrl(url).Result;

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response);

            var allPictures = htmlDoc.DocumentNode.Descendants("img");
            var allPictures2 = htmlDoc.DocumentNode.SelectNodes(".//img");
            return Ok(response);
        }

        [HttpPost("zara")]
        public async Task<ActionResult> crawlZara(LoginDto loginDto)
        {
            var url = "https://www.zara.com/rs/";
            var response = CallUrl(url).Result;

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response);
            //return Ok(response);
            List<string> narrowedLinks = new List<string>();

            var links = htmlDoc.DocumentNode.Descendants("a").Where(node => node.GetAttributeValue("class", "").Contains("layout-categories-category__link")).ToList();
            foreach (var node in links.ElementAt<HtmlNode>(2).NextSibling.ChildNodes)
            {
                if (node.Name == "li")
                {
                    if (node.FirstChild.Attributes[1].Value != "#")
                    {
                        narrowedLinks.Add(node.FirstChild.Attributes[1].Value);
                    }
                }
            }
            narrowedLinks.RemoveAt(narrowedLinks.Count - 1);
            foreach (var node in links.ElementAt<HtmlNode>(11).NextSibling.ChildNodes)
            {
                if (node.Name == "li")
                {
                    if (node.FirstChild.Attributes[1].Value != "#")
                    {
                        narrowedLinks.Add(node.FirstChild.Attributes[1].Value);
                    }
                }
            }
            narrowedLinks.RemoveAt(narrowedLinks.Count - 1);

            //DOHVATANJE ARTIKALA
            List<ArticleDto> articles = new List<ArticleDto>();
            var faileds = 0;
            foreach (var str in narrowedLinks)
            {
                response = CallUrl(str).Result;
                htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response);

                var picturesLinks = htmlDoc.DocumentNode.Descendants("a").Where(node => node.GetAttributeValue("class", "").Contains("product-link product-grid-product__link link")).ToList();

                string type = zaraDetermineCategory(str);
                char gender = 'M';
                if (str.IndexOf("woman") >= 0)
                {
                    gender = 'F';
                }


                foreach (var pic in picturesLinks)
                {
                    try
                    {
                        ArticleDto newArticle = new ArticleDto { };
                        newArticle.href = pic.Attributes[1].Value;
                        newArticle = await this.zaraGetArticleInfo(newArticle);
                        newArticle.type = type;
                        newArticle.gender = gender;
                        articles.Add(newArticle);
                    }
                    catch (Exception e)
                    {
                        ++faileds;
                    }
                }
                Console.WriteLine("ZAVRSENO " + str);
            }


            Console.WriteLine("ZAVRSENO SVE uz " + faileds + " propalih artikala");
            return Ok("articles.ToArray()");

        }

        [HttpPost("PullAndBear")]
        public ActionResult crawlPullAndBear(ArticleDto articleDto)
        {

            var url = "https://www.pullandbear.com/rs/muskarci-n6228";
            var url2 = "https://www.pullandbear.com/rs/zene-n6417";
            var response = CallUrl(url).Result;
            var response2 = CallUrl(url2).Result;

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response);

            var links = htmlDoc.DocumentNode.Descendants("a").Where(node => node.GetAttributeValue("class", "").Contains("c-main-nav-link spa-ready")).ToList<HtmlNode>();


            return Ok(response);
        }

        [HttpPost("zaraImage")]
        public async Task<ActionResult> zaraGetPuppeter(ArticleDto articleDto)
        {

            //var fullUrl = "https://www.bershka.com/rs/%C5%BEene/ode%C4%87a/majice-i-topovi-c1010193217.html";
            // var fullUrl = "https://www.zara.com/rs/sr/woman-blazers-l1055.html?v1=1882227";
            var fullUrl = articleDto.href;
            var response = await CallUrl(fullUrl);

            var options = new LaunchOptions()
            {
                Headless = true,
                ExecutablePath = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe",
                Product = Product.Chrome
            };
            var browser = await Puppeteer.LaunchAsync(options, null);
            var page = await browser.NewPageAsync();
            await page.GoToAsync(fullUrl, new NavigationOptions()
            {
                WaitUntil = new WaitUntilNavigation[] { WaitUntilNavigation.Load, WaitUntilNavigation.DOMContentLoaded, WaitUntilNavigation.Networkidle0, WaitUntilNavigation.Networkidle2 },
                Timeout = 0
            });
            var links = @"Array.from(document.querySelectorAll('img')).map(img => img.src);";
            var urls = await page.EvaluateExpressionAsync<string[]>(links);

            return Ok("LOL");
        }


        [HttpPost("helper")]
        public async Task<ActionResult> helper(ArticleDto details)
        {

            var fullUrl = details.href;
            var response = await CallUrl(fullUrl);

            return Ok(response);
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response);
            // var price = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='price__amount-current'").InnerHtml;
            var price = htmlDoc.DocumentNode.Descendants("span").Where(node => node.GetAttributeValue("class", "").Equals("price__amount-current")).First().InnerHtml;
            var title = htmlDoc.DocumentNode.Descendants("h1").Where(node => node.GetAttributeValue("class", "").Equals("product-detail-card-info__name")).First().InnerHtml;
            details.price = price;
            details.name = title;
            //var images = htmlDoc.DocumentNode.Descendants("picture").Where(node => node.GetAttributeValue("class", "").Equals("media-image")).First();
            int i = 0;
            string parse = response;
            List<string> slike = new List<string>();
            while ((i = parse.IndexOf("<picture")) >= 0)
            {
                i += 8;
                parse = parse.Substring(i);
                i = parse.IndexOf("srcSet=") + 8;
                string temp = "";
                while (!(parse[i].Equals(' ')))
                {
                    temp += parse[i];
                    i++;
                }
                Console.WriteLine(temp);
                slike.Add(temp);
                //details.imgSrc.
            }
            details.imgSrc = slike.ToArray();

            return Ok(details);
        }

        private string zaraDetermineCategory(string url)
        {
            int i = url.IndexOf("man-");
            if (i == -1)
            {
                return "other";
            }
            i += 4;
            string type = "";
            while (!url[i].Equals('-'))
            {
                type += url[i];
                i++;
            }
            Console.WriteLine(type);
            return type;
        }

        private async Task<ArticleDto> zaraGetArticleInfo(ArticleDto details)
        {

            var fullUrl = details.href;
            var response = await CallUrl(fullUrl);
            //return Ok(response);

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response);
            var price = htmlDoc.DocumentNode.Descendants("span").Where(node => node.GetAttributeValue("class", "").Equals("price__amount-current")).First().InnerHtml;
            var title = htmlDoc.DocumentNode.Descendants("h1").Where(node => node.GetAttributeValue("class", "").Equals("product-detail-card-info__name")).First().InnerHtml;

            details.price = price;
            details.name = title;

            int i = 0;
            string parse = response;
            List<string> slike = new List<string>();
            while ((i = parse.IndexOf("<picture")) >= 0)
            {
                i += 8;
                parse = parse.Substring(i);
                i = parse.IndexOf("srcSet=") + 8;
                string temp = "";
                while (!(parse[i].Equals(' ')))
                {
                    temp += parse[i];
                    i++;
                }
                //Console.WriteLine(temp);
                slike.Add(temp);
            }
            details.imgSrc = slike.ToArray();

            return details;
        }



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