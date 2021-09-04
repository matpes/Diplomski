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
    using API.Entitites;
    using API.Interfaces;

    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;

    public class CrawlerController : BaseApiController
    {
        private readonly IArticlesRepository _articlesRepository;

        private static readonly LaunchOptions puppeteerLaunchoptions = new LaunchOptions()
        {
            Headless = true,
            ExecutablePath = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe",
            Product = Product.Chrome
        };


        private static readonly NavigationOptions puppeteerNavigationOptions = new NavigationOptions()
        {
            WaitUntil = new WaitUntilNavigation[] { WaitUntilNavigation.Load, WaitUntilNavigation.DOMContentLoaded, WaitUntilNavigation.Networkidle0, WaitUntilNavigation.Networkidle2 },
            Timeout = 0
        };

        private static readonly NavigationOptions fasterPuppeteerNavigationOptions = new NavigationOptions()
        {
            WaitUntil = new WaitUntilNavigation[] { WaitUntilNavigation.Load, WaitUntilNavigation.Networkidle0, WaitUntilNavigation.Networkidle2 },
            Timeout = 0
        };

        public CrawlerController(IArticlesRepository articlesRepository)
        {
            _articlesRepository = articlesRepository;
        }



        //DOHVACENI SU LINKOVI KA POSEBNIM KATEGORIJAMA
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



        [HttpGet("PullAndBear")]
        public async Task<ActionResult> crawlPullAndBear()
        {

            var url = "https://www.pullandbear.com/rs/muskarci-n6228";
            var url2 = "https://www.pullandbear.com/rs/zene-n6417";

            var links = @"Array.from(document.querySelectorAll('a')).map(a => a.href);";

            var browser = await Puppeteer.LaunchAsync(puppeteerLaunchoptions, null);

            //MEN
            var page = await browser.NewPageAsync();
            await page.GoToAsync(url, puppeteerNavigationOptions);
            var urls = await page.EvaluateExpressionAsync<string[]>(links);

            //WOMEN
            var page2 = await browser.NewPageAsync();
            await page2.GoToAsync(url2, puppeteerNavigationOptions);
            var urls2 = await page2.EvaluateExpressionAsync<string[]>(links);

            List<string> men = getPullAndBearLinksLevel1("/muskarci/odeca/", urls);
            List<string> women = getPullAndBearLinksLevel1("/zene/odeca/", urls2);

            List<ArticleDto> articlesList = new List<ArticleDto>();
            articlesList = await getPullAndBearArticles('M', men, articlesList);
            articlesList = await getPullAndBearArticles('F', women, articlesList);

            _articlesRepository.insertArrticles(articlesList);

            return Ok(articlesList);

        }

        private string getPullAndBearCategory(string str)
        {
            int i = str.LastIndexOf('/') + 1;
            int j = str.LastIndexOf("-n");
            string ret;
            ret = str.Substring(i, j - i);
            return ret;
        }

        private List<string> getPullAndBearLinksLevel1(string param, string[] urls)
        {
            List<string> list = new List<string>();

            foreach (var link in urls.Distinct())
            {
                if (!list.Contains(link) && link.Contains(param))
                {
                    int firstInex = link.IndexOf(param) + param.Length - 1;
                    int lastIndex = link.LastIndexOf('/');
                    int hasQuestionMark = link.IndexOf("?celement");
                    if (firstInex == lastIndex && hasQuestionMark == -1)
                    {
                        list.Add(link);
                    }
                }
            }

            return list;
        }

        private async Task<List<ArticleDto>> getPullAndBearArticles(char gender, List<string> women, List<ArticleDto> articlesList)
        {
            foreach (var link in women)
            {
                string type = getPullAndBearCategory(link);

                var response = await CallUrl(link);

                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response);

                var linksA = htmlDoc.DocumentNode.Descendants("a").ToList();

                foreach (var a in linksA)
                {
                    if (!a.FirstChild.Name.Equals("img"))
                    {
                        break;
                    }
                    ArticleDto article = new ArticleDto { };
                    article.gender = gender;
                    article.type = type;
                    article.href = a.Attributes[0].Value;
                    article.name = a.ChildNodes[1].InnerHtml;
                    article.price = a.ChildNodes[2].InnerHtml + " RSD";
                    var image = a.FirstChild.Attributes[0].Value;

                    if (articlesList.FindIndex(x => x.href.Equals(article.href)) == -1)
                    {
                        article = await getPullAndBearImagesForArticle(article, image);
                        articlesList.Add(article);
                    }
                }
                Console.WriteLine("Finished " + link);
            }
            return articlesList;
        }

        private async Task<ArticleDto> getPullAndBearImagesForArticle(ArticleDto article, string linkic)
        {
            int j = 0;
            string path = linkic.Substring(0, linkic.IndexOf('?'));
            var indexFirst = path.IndexOf("_");
            var indexLast = path.LastIndexOf("_");
            string part1 = path.Substring(0, indexFirst + 1);
            string part2 = path.Substring(indexLast);
            List<ArticleImagesDto> list = new List<ArticleImagesDto>();
            for (int i = 1; i < 9; i++)
            {
                string link = part1 + "2_" + i + part2;
                ArticleImagesDto images = new ArticleImagesDto { };
                images.src = link;
                try{
                    await CallUrl(link);
                    list.Add(images);
                }catch(Exception){
                    
                    if(++j==2){
                        break;
                    }
                }

            }
            if (list.Count == 0)
            {
                ArticleImagesDto images = new ArticleImagesDto { };
                images.src = linkic;
                list.Add(images);
            }

            article.imgSources = list;
            return article;
        }

        [HttpPost("testPictures")]
        public async Task<ActionResult<ICollection<ArticleImagesDto>>> getPullAndBearImagesForArticle(LoginDto links)
        {

            /*var url = links.Username;
            var imageStartingString = links.Password;
            var endIndex = imageStartingString.IndexOf(".jpg") + 4;
            imageStartingString = imageStartingString.Substring(0, endIndex);
            var ids = @"Array.from(document.querySelectorAll('img')).map(img => img.id);";
            var browser = await Puppeteer.LaunchAsync(puppeteerLaunchoptions, null);
            //MEN
            var page = await browser.NewPageAsync();
            await page.GoToAsync(url, fasterPuppeteerNavigationOptions);
            var picturesIds = await page.EvaluateExpressionAsync<string[]>(ids);

            var found = false;
            var originId = "";
            var indexStart = 0;
            var indexEnd = 0;
            foreach (var id in picturesIds)
            {
                if ((!("").Equals(id)) && imageStartingString.Contains(id))
                {
                    found = true;
                    originId = id;
                    indexStart = imageStartingString.IndexOf(id);
                    indexEnd = indexStart + id.Length;
                    break;
                }
            }
            List<ArticleImagesDto> list = new List<ArticleImagesDto>();
            if (!found)
            {
                ArticleImagesDto images = new ArticleImagesDto { };
                images.src = links.Password;
                list.Add(images);
                return Ok("NO");
            }
            //TO DO: RETURN ERROR IF FOUND STILL FALSE


            foreach (var id in picturesIds)
            {
                if (!id.Equals(""))
                {
                    ArticleImagesDto images = new ArticleImagesDto { };
                    images.src = imageStartingString.Substring(0, indexStart);
                    images.src += id;
                    images.src += imageStartingString.Substring(indexEnd);
                    list.Add(images);
                }
            }


            return Ok(list);*/

            int j = 0;

            string path = links.Password;
            var indexFirst = path.IndexOf("_");
            var indexLast = path.LastIndexOf("_");
            string part1 = path.Substring(0, indexFirst + 1);
            string part2 = path.Substring(indexLast);
            List<ArticleImagesDto> list = new List<ArticleImagesDto>();
            for (int i = 1; i < 10; i++)
            {
                string link = part1 + "2_" + i + part2;
                try
                {
                    var response = await CallUrl(link);
                    ArticleImagesDto images = new ArticleImagesDto { };
                    images.src = link;
                    list.Add(images);
                    j = 0;
                }
                catch (Exception)
                {
                    j++;
                    if (j == 2)
                    {
                        break;
                    }
                }

            }

            return Ok(list);
        }

        /**
        Metoda za krolovanje Zarinog sajta.
        RADI.

        TO DO: EVENTUALNO DODATI I DOHVATANJE BOJE ARTIKLA

        TO DO: PROVERITI DA LI ARTIKALA VEC IMA U BAZI

        PARAMS:
        RETURNS: ActionResult

        */
        [HttpGet("zara")]
        public async Task<ActionResult> crawlZara()
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
            foreach (var node in links.ElementAt<HtmlNode>(12).NextSibling.ChildNodes)
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
            foreach (var str in narrowedLinks.Distinct())
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
                        newArticle = await zaraGetArticleInfo(newArticle);
                        newArticle.type = type;
                        newArticle.gender = gender;
                        if (articles.FindIndex(x => x.href.Equals(newArticle.href)) == -1)
                        {
                            articles.Add(newArticle);
                        }
                    }
                    catch (Exception)
                    {
                        ++faileds;
                    }
                }
            }

            _articlesRepository.insertArrticles(articles);


            Console.WriteLine("ZAVRSENO SVE uz " + faileds + " propalih artikala");
            return Ok("articles.ToArray()");

        }

        //OVO SE NE SECAM STA JE
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


        [HttpGet("helper")]
        public ActionResult helper()
        {
            return Ok("sb.ToString()");
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
            ICollection<ArticleImagesDto> slike = new List<ArticleImagesDto>();
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
                if (!temp.Contains("https://static.zara.net/photos///contents/cm/sustainability/extrainfo/w/563/"))
                {
                    ArticleImagesDto image = new ArticleImagesDto();
                    image.src = temp;
                    slike.Add(image);
                }
            }
            details.imgSources = slike;

            return details;
        }



        private static async Task<string> CallUrl(string fullUrl)
        {
            HttpClient client = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
            client.DefaultRequestHeaders.Accept.Clear();
            var response = await client.GetStringAsync(fullUrl);
            return response;
        }

        private static async Task<bool> CheckUrl(string fullUrl)
        {
            var client = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
            client.DefaultRequestHeaders.Accept.Clear();
            var response = await client.GetAsync(fullUrl);
            return response.IsSuccessStatusCode;
        }

    }
}