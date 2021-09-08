namespace API.Controllers
{
    using HtmlAgilityPack;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Net;
    using System.Text;
    using Microsoft.AspNetCore.Mvc;
    using API.DTOs;
    using System.Linq;
    using System.Collections.Generic;
    using System;

    using PuppeteerSharp;
    using API.Interfaces;



    public class CrawlerController : BaseApiController
    {

        private static Dictionary<string, string> PullAndBearCategoryDictionary = new Dictionary<string, string>{
            {"kombinezoni-i-pantalone-na-tregere", "kombinezoni"},
            {"kratke-pantalone", "sortsevi"},
            {"kupaci-kostimi", "odeca-za-plazu"},
            {"pletenina", "pletena-odeca"},
            {"jakne-i-sakoi", "kaputi-i-jakne"},
            {"bluze-i-kosulje", "kosulje"},
            {"pakovanja", "ostalo"},
            {"intimates", "ostalo"},
            {"kupace-gace", "odeca-za-plazu"},
            {"jakne", "kaputi-i-jakne"},
            {"bermude", "sortsevi"},
            {"cool-jeans", "farmerke"}
        };

        private static Dictionary<string, string> ZaraCategoryDictionary = new Dictionary<string, string>{
            {"jumpsuits", "kombinezoni"},
            {"bermudas", "sortsevi"},
            {"knitwear", "pletena-odeca"},
            {"blazers", "kaputi-i-jakne"},
            {"shirts", "kosulje"},
            {"jeans", "farmerke"},
            {"jackets", "kaputi-i-jakne"},
            {"outerwear", "kaputi-i-jakne"},
            {"sweatshirts", "dukserice"},
            {"tshirts", "majice"},
            {"polos", "majice"},
            {"trousers", "pantalone"},
            {"tops", "topovi"},
            {"dresses", "haljine"},
            {"skirts", "suknje"},
            {"suits", "odlea"}
        };

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


        /**
        ===================================
                    TERRANOVA
        ===================================
        Metoda za krolovanje PullAndBear sajta.
        RADI.

        VREME IZVRSAVANJA: Od 2 - 20 minuta
        TO DO: PROVERITI DA LI ARTIKALA VEC IMA U BAZI

        PARAMS:
        RETURNS: ActionResult
        */
        [HttpGet("terranova")]
        public async Task<ActionResult> crawlTerranova()
        {
            var url = "https://www.terranovastyle.com/rs_sr/musko/odeca/";
            var response = await CallUrl(url);
            var j = 0;

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response);
            var links = htmlDoc.DocumentNode.Descendants("a").Where(node => node.GetAttributeValue("class", "").Contains("js-activateSublevel")).ToList();
            List<string> newLinks = new List<string>();
            for (var i = 2; i < 18; i++)
            {
                newLinks.Add(links.ElementAt<HtmlNode>(i).Attributes[2].Value);
            }
            for (var i = 29; i < 40; i++)
            {
                newLinks.Add(links.ElementAt<HtmlNode>(i).Attributes[2].Value);
            }
            //OBILAZAK PO KATEGORIJI
            List<ArticleDto> articlesList = new List<ArticleDto>();
            foreach (var allArticlesSublink in newLinks)
            {
                var gender = 'M';
                if (allArticlesSublink.Contains("/zensko/"))
                {
                    gender = 'F';
                }
                var type = getTerranovaCategory(allArticlesSublink);
                response = await CallUrl(allArticlesSublink);
                htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response);
                var imageLinks = htmlDoc.DocumentNode.Descendants("a").Where(node => node.GetAttributeValue("class", "").Contains("product-item-photo")).ToList();
                //var imageSources = htmlDoc.DocumentNode.Descendants("img").Where(node => node.GetAttributeValue("class", "").Contains("product-image-photo")).ToList();
                foreach (var articleHref in imageLinks)
                {
                    ArticleDto article = new ArticleDto { };
                    article.gender = gender;
                    article.type = type;
                    article.href = articleHref.Attributes[0].Value;
                    article.storeId = 3;
                    if (articlesList.FindIndex(x => x.href.Equals(article.href)) == -1)
                    {
                        try
                        {

                            article = getTerranovaImagesForArticle(article);
                            articlesList.Add(article);
                        }
                        catch (Exception)
                        {
                            ++j;
                        }
                    }
                }

                Console.WriteLine("Finnished " + allArticlesSublink);

            }
            Console.WriteLine("Finnished with " + j + " failed articles");
            _articlesRepository.insertArrticles(articlesList);
            return Ok(articlesList);

        }

        private ArticleDto getTerranovaImagesForArticle(ArticleDto article)
        {
            var url = article.href;
            var response = CallUrl(url).Result;
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(response);

            var images = htmlDocument.DocumentNode.Descendants("img").Where(node => node.GetAttributeValue("class", "").Contains("img-responsive")).ToList();
            var price = htmlDocument.DocumentNode.Descendants("span").Where(node => node.GetAttributeValue("class", "").Contains("price-wrapper")).ToList();
            var name = htmlDocument.DocumentNode.Descendants("h1").Where(node => node.GetAttributeValue("class", "").Contains("page-title")).ToList();
            var priceString = price.ElementAt(price.Count - 1).GetAttributeValue("content", "2990");
            article.price = int.Parse(priceString);
            article.name = name.ElementAt(0).ChildNodes[1].InnerText;
            List<ArticleImagesDto> list = new List<ArticleImagesDto>();
            var i = 0;
            foreach (var image in images)
            {
                ArticleImagesDto img = new ArticleImagesDto { };
                var src = image.GetAttributeValue("src", "");
                if (src.Equals(""))
                {
                    continue;
                }
                else
                {
                    if (src[src.Length - 1].Equals('/'))
                    {
                        src = src.Remove(src.Length - 1);
                    }
                    var index = src.IndexOf("/it_it/");
                    if (index != -1)
                    {
                        src = src.Remove(index, 6);
                    }
                    img.src = src;
                    list.Add(img);
                    ++i;
                }
                if (i == 5)
                {
                    break;
                }
            }
            article.imgSources = list;
            return article;
        }

        private string getTerranovaCategory(string url)
        {
            var index = url.IndexOf("/odeca/") + 7;
            StringBuilder sb = new StringBuilder();
            while (!url[index].Equals('/'))
            {
                sb.Append(url[index]);
                ++index;
            }
            var kat = sb.ToString();
            if (kat.Equals("bodi") || kat.Equals("majice-dugih-rukava"))
            {
                kat = "majice";
            }
            else if (kat.Equals("helanke") || kat.Equals("sportske-pantalone"))
            {
                kat = "pantalone";
            }
            else if (kat.Equals("prsluci"))
            {
                kat = "kaputi-i-jakne";
            }
            return kat;
        }





        /**
        ======================================
                        P&B
        =====================================
        Metoda za krolovanje PullAndBear sajta.
        RADI.

        VREME IZVRSAVANJA: Oko 35 minuta
        TO DO: PROVERITI DA LI ARTIKALA VEC IMA U BAZI

        PARAMS:
        RETURNS: ActionResult

        */
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
            try
            {
                articlesList = await getPullAndBearArticles('M', men, articlesList);
            }
            catch (Exception) { }
            try
            {
                articlesList = await getPullAndBearArticles('F', women, articlesList);
            }
            catch (Exception) { }
            _articlesRepository.insertArrticles(articlesList);

            return Ok(articlesList);

        }

        private string getPullAndBearCategory(string str)
        {
            int i = str.LastIndexOf('/') + 1;
            int j = str.LastIndexOf("-n");
            string ret;
            ret = str.Substring(i, j - i);
            string output;
            if (!PullAndBearCategoryDictionary.TryGetValue(ret, out output))
            {
                output = ret;
            }
            return output;
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
                try
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
                        var price = a.ChildNodes[2].InnerHtml;
                        var image = a.FirstChild.Attributes[0].Value;
                        if (price.IndexOf(" - ") != -1)
                        {
                            price = price.Remove(price.IndexOf(" - "));
                        }
                        article.price = int.Parse(price);
                        if (articlesList.FindIndex(x => x.href.Equals(article.href)) == -1)
                        {
                            article = await getPullAndBearImagesForArticle(article, image);
                            articlesList.Add(article);
                        }
                    }
                    Console.WriteLine("Finished " + link);
                }
                catch (Exception) { }
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
                try
                {
                    await CallUrl(link);
                    list.Add(images);
                }
                catch (Exception)
                {

                    if (++j == 2)
                    {
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



        /**
        ======================================
                        ZARA
        =====================================
        Metoda za krolovanje Zarinog sajta.
        RADI.

        VREME IZVRSAVANJA: Oko 10 minuta
        TO DO: ISPRAVITI HARDKODOVANO DOHVATANJE KATEGORIJE 
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


        private string zaraDetermineCategory(string url)
        {
            int i = url.IndexOf("man-");
            if (i == -1)
            {
                return "ostalo";
            }
            i += 4;
            string type = "";
            while (!url[i].Equals('-'))
            {
                type += url[i];
                i++;
            }

            string ret;
            if (!ZaraCategoryDictionary.TryGetValue(type, out ret))
            {
                ret = "ostalo";
            }
            return ret;
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

            price = price.Replace(" RSD", "");
            price = price.Replace(".", "");

            details.price = int.Parse(price);
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

        //UNUSED
        //DOHVACENI SU LINKOVI KA POSEBNIM KATEGORIJAMA
        [HttpPost("bershka")]
        public async Task<ActionResult> crawlBershka(LoginDto loginDto)
        {
            //Ovde se i sa zensog i sa muskog startnog sajta vide svi linkovi
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
                        Console.WriteLine(links.ElementAt<HtmlNode>(i).InnerText);
                    }
                }
                i++;
            }

            //Imamo spremne sve linkove (svih 20)
            //DOVDE SVE RADI


            var linksQueryString = @"Array.from(document.querySelectorAll('a')).map(a => a.href);";

            var browser = await Puppeteer.LaunchAsync(puppeteerLaunchoptions, null);
            var page = await browser.NewPageAsync();


            foreach (var newUrl in narrowedLinks)
            {
                await page.GoToAsync(newUrl, fasterPuppeteerNavigationOptions);
                var urls = await page.EvaluateExpressionAsync<string[]>(linksQueryString);


                // response = CallUrl(newUrl).Result;

                // htmlDoc = new HtmlDocument();
                // htmlDoc.LoadHtml(response);

                //var linkovi = htmlDoc.DocumentNode.Descendants("a").ToList();

                return Ok(urls);

            }

            return null;
        }

        [HttpGet("prices")]
        public void updatePrices()
        {
            _articlesRepository.specialMethod();
        }



    }
}