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


        //OVDE JE NESTO RADJENO, ALI SE NE SECAM STA TACNO
        //DOHVATANE SU SLIKE ZA PULL AND BEAR
        [HttpPost("pullAndBearDetalj")]
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


        [HttpPost("PullAndBear")]
        public async Task<ActionResult> crawlPullAndBear(LoginDto login)
        {
            /*
            var url = login.Username;
            var url2 = login.Password;

            var links = @"Array.from(document.querySelectorAll('a')).map(a => a.href);";

            var browser = await Puppeteer.LaunchAsync(puppeteerLaunchoptions, null);

            //MEN
            var page = await browser.NewPageAsync();
            await page.GoToAsync(url, puppeteerNavigationOptions);
            var urls = await page.EvaluateExpressionAsync<string[]>(links);
            Console.WriteLine("WENT AND DONE MEN " + urls.Count());

            //WOMEN
            var page2 = await browser.NewPageAsync();
            await page2.GoToAsync(url2, puppeteerNavigationOptions);
            var urls2 = await page2.EvaluateExpressionAsync<string[]>(links);
            Console.WriteLine("WENT AND DONE WOMEN " + urls2.Count());

            List<string> men = getPullAndBearLinksLevel1("/muskarci/odeca/", urls);
            List<string> women = getPullAndBearLinksLevel1("/zene/odeca/", urls2);

            List<ArticleDto> newArticles = new List<ArticleDto>();

            foreach (var man in men)
            {
                ArticleDto article = new ArticleDto { };
                article.gender = 'M';
                article.type = getPullAndBearCategory(man);

            }

            Console.WriteLine("Muskih linkova ima: " + men.Count + ", a zenskih ima: " + women.Count);
            */
            string[] PullAndBearLinks = {
                "https://www.pullandbear.com/rs/muskarci/odeca/farmerke-n6347",
                "https://www.pullandbear.com/rs/muskarci/odeca/pantalone-n6363",
                "https://www.pullandbear.com/rs/muskarci/odeca/majice-n6323",
                "https://www.pullandbear.com/rs/muskarci/odeca/polo-majice-n6371",
                "https://www.pullandbear.com/rs/muskarci/odeca/dukserice-n6382",
                "https://www.pullandbear.com/rs/muskarci/odeca/jakne-n6335",
                "https://www.pullandbear.com/rs/muskarci/odeca/bermude-n6308",
                "https://www.pullandbear.com/rs/muskarci/odeca/kosulje-n6313",
                "https://www.pullandbear.com/rs/muskarci/odeca/kupace-gace-n6299",
                "https://www.pullandbear.com/rs/muskarci/odeca/pletenina-n6372",
                "https://www.pullandbear.com/rs/zene/odeca/intimates-n7128",
                "https://www.pullandbear.com/rs/zene/odeca/farmerke-n6581",
                "https://www.pullandbear.com/rs/zene/odeca/cool-jeans-n7044",
                "https://www.pullandbear.com/rs/zene/odeca/pantalone-n6600",
                "https://www.pullandbear.com/rs/zene/odeca/majice-n6541",
                "https://www.pullandbear.com/rs/zene/odeca/topovi-n6644",
                "https://www.pullandbear.com/rs/zene/odeca/dukserice-n6636",
                "https://www.pullandbear.com/rs/zene/odeca/haljine-n6646",
                "https://www.pullandbear.com/rs/zene/odeca/jakne-i-sakoi-n6555",
                "https://www.pullandbear.com/rs/zene/odeca/bluze-i-kosulje-n6525",
                "https://www.pullandbear.com/rs/zene/odeca/kratke-pantalone-n6629",
                "https://www.pullandbear.com/rs/zene/odeca/suknje-n6571",
                "https://www.pullandbear.com/rs/zene/odeca/kombinezoni-i-pantalone-na-tregere-n6599",
                "https://www.pullandbear.com/rs/zene/odeca/pletenina-n6618",
                "https://www.pullandbear.com/rs/zene/odeca/kupaci-kostimi-n6513",
                "https://www.pullandbear.com/rs/zene/odeca/pakovanja-n6979"
            };

            foreach (var link in PullAndBearLinks)
            {
                ArticleDto article = new ArticleDto { };
                //char gender = 'M';
                //string type = getPullAndBearCategory(link);

                var links = @"Array.from(document.querySelectorAll('img')).map(img => img.src);";

                var browser = await Puppeteer.LaunchAsync(puppeteerLaunchoptions, null);

                //MEN
                var page = await browser.NewPageAsync();
                await page.GoToAsync(link, puppeteerNavigationOptions);
                var pics = await page.EvaluateExpressionAsync<string[]>(links);

                StringBuilder sb = new StringBuilder();
                foreach(var l in pics){
                    sb.Append(l).AppendLine();
                }
                return Ok(sb.ToString());
                //Console.WriteLine("WENT AND DONE MEN " + urls.Count());

            }

            return Ok("LOL");

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
            string[] PullAndBearLinks = {
                "https://www.pullandbear.com/rs/muskarci/odeca/farmerke-n6347",
                "https://www.pullandbear.com/rs/muskarci/odeca/pantalone-n6363",
                "https://www.pullandbear.com/rs/muskarci/odeca/majice-n6323",
                "https://www.pullandbear.com/rs/muskarci/odeca/polo-majice-n6371",
                "https://www.pullandbear.com/rs/muskarci/odeca/dukserice-n6382",
                "https://www.pullandbear.com/rs/muskarci/odeca/jakne-n6335",
                "https://www.pullandbear.com/rs/muskarci/odeca/bermude-n6308",
                "https://www.pullandbear.com/rs/muskarci/odeca/kosulje-n6313",
                "https://www.pullandbear.com/rs/muskarci/odeca/kupace-gace-n6299",
                "https://www.pullandbear.com/rs/muskarci/odeca/pletenina-n6372",
"https://www.pullandbear.com/rs/zene/odeca/intimates-n7128",
"https://www.pullandbear.com/rs/zene/odeca/farmerke-n6581",
"https://www.pullandbear.com/rs/zene/odeca/cool-jeans-n7044",
"https://www.pullandbear.com/rs/zene/odeca/pantalone-n6600",
"https://www.pullandbear.com/rs/zene/odeca/majice-n6541",
"https://www.pullandbear.com/rs/zene/odeca/topovi-n6644",
"https://www.pullandbear.com/rs/zene/odeca/dukserice-n6636",
"https://www.pullandbear.com/rs/zene/odeca/haljine-n6646",
"https://www.pullandbear.com/rs/zene/odeca/jakne-i-sakoi-n6555",
"https://www.pullandbear.com/rs/zene/odeca/bluze-i-kosulje-n6525",
"https://www.pullandbear.com/rs/zene/odeca/kratke-pantalone-n6629",
"https://www.pullandbear.com/rs/zene/odeca/suknje-n6571",
"https://www.pullandbear.com/rs/zene/odeca/kombinezoni-i-pantalone-na-tregere-n6599",
"https://www.pullandbear.com/rs/zene/odeca/pletenina-n6618",
"https://www.pullandbear.com/rs/zene/odeca/kupaci-kostimi-n6513",
"https://www.pullandbear.com/rs/zene/odeca/pakovanja-n6979"
            };

            StringBuilder sb = new StringBuilder();
            foreach (var man in PullAndBearLinks)
            {

                sb.Append(getPullAndBearCategory(man)).Append("\n");

            }

            return Ok(sb.ToString());

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

    }
}