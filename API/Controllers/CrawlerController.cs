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
        public ActionResult crawlZara(LoginDto loginDto)
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
            Console.WriteLine("ZAVRSEN WOMAN");
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

            Console.WriteLine("ZAVRSEN MAN");
            narrowedLinks.RemoveAt(narrowedLinks.Count - 1);

            // StringBuilder sb = new StringBuilder();
            // foreach(var str in narrowedLinks){
            //     sb.Append(str).Append("\n");
            //     //Console.WriteLine(str);
            // }

            foreach (var str in narrowedLinks)
            {
                response = CallUrl(str).Result;

                htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response);

                //var itemPrices = htmlDoc.DocumentNode.Descendants("span").Where(node => node.GetAttributeValue("class", "").Contains("price__amount-current")).ToList();
                //var itemImages = htmlDoc.DocumentNode.Descendants("img").Where(node => node.GetAttributeValue("class", "").Contains("media-image__image")).ToList();
                //var itemLink1 = htmlDoc.DocumentNode.Descendants("a").Where(node => node.GetAttributeValue("class", "").Contains("product-link")).ToList();
                //var itemLink2 = htmlDoc.DocumentNode.Descendants("a").Where(node => node.GetAttributeValue("class", "").Contains("product-grid-product__link")).ToList();

                var productsTypeOne = htmlDoc.DocumentNode.Descendants("li").Where(node => node.GetAttributeValue("class", "").Contains("product-grid-block")).ToList();
                var picturesLinks = htmlDoc.DocumentNode.Descendants("a").Where(node => node.GetAttributeValue("class", "").Contains("product-link product-grid-product__link link")).ToList();
                var allPictures = htmlDoc.DocumentNode.Descendants("img");
                var allPictures2 = htmlDoc.DocumentNode.SelectNodes(".//img");
                //List<HtmlNode> chosenOnes = new List<HtmlNode>();
                List<ArticleDto> articles = new List<ArticleDto>();

                foreach (var pic in picturesLinks)
                {
                    ArticleDto newArticle = new ArticleDto { };
                    var href = pic.Attributes[1].Value;
                    newArticle.href = href;
                    // var imageList = pic.Descendants("img").Where(node => node.GetAttributeValue("class", "").Contains("media-image__image")).ToList();
                    var imageList = pic.Descendants("img").ToList();
                    if (imageList.Count != 2)
                    {
                        continue;
                    }
                    else
                    {
                        var image1 = imageList.ElementAt(0);
                        var image2 = imageList.ElementAt(1);
                        newArticle.name = image1.Attributes[1].Value;
                        newArticle.imgSrc = image2.Attributes[0].Value;
                        articles.Add(newArticle);
                    }
                }

                var descriptionLinks = htmlDoc.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Contains("product-grid-product-info")).ToList();

                foreach (var desc in descriptionLinks)
                {
                    var href = "";
                    var aList = desc.Descendants("a").ToList();
                    if (aList.Count == 0)
                    {
                        continue;
                    }
                    else
                    {
                        var a = aList.ElementAt(0);
                        href = a.Attributes[1].Value;
                        ArticleDto existingArticle = articles.Find(art => art.href.Equals(href));
                        if (existingArticle != null)
                        {
                            var price = desc.Descendants("span").Where(node => node.GetAttributeValue("class", "").Equals("price__amount-current")).First<HtmlNode>();
                            if (price != null)
                            {
                                existingArticle.price = price.InnerHtml;
                            }
                        }
                    }
                }

                // foreach (var product in productsTypeOne)
                // {
                //     var priceOfProduct = product.Descendants("span").Where(node => node.GetAttributeValue("class", "").Contains("price__amount-current")).ToList();
                //     var imageOfProduct = product.Descendants("img").Where(node => node.GetAttributeValue("class", "").Contains("media-image__image")).ToList();
                //     var linkOfProduct = product.Descendants("a").Where(node => node.GetAttributeValue("class", "").Contains("product-link")).ToList();
                //     if (imageOfProduct.Count > 0){
                //         HtmlNode image = imageOfProduct.ElementAt(0);
                //         articles.
                //     }else if(linkOfProduct.Count > 0){

                //     }
                //     if (imageOfProduct.Count > 0)
                //     {
                //         ++i;
                //         //chosenOnes.Add(product);
                //         // Console.WriteLine(imageOfProduct.ElementAt<HtmlNode>(0).Attributes[1].Value);
                //         // Console.WriteLine(imageOfProduct.ElementAt<HtmlNode>(0).Attributes[2].Value);
                //     }else{
                //         j++;
                //     }
                // }

                return Ok("response");
            }



            Console.WriteLine("ZAVRSENO SVE");
            return Ok("RADI");

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