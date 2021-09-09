using System;
using System.Threading;
using API.Controllers;
using API.Data;
using API.Interfaces;

namespace API.Services
{
    public class CrawlerService
    {
        public void Crawl()
        {

            Console.WriteLine("Nit za crawlovanje je startovana");
            while (true)
            {
                TimeSpan timeSpan = new TimeSpan(24, 0, 0);
                Thread.Sleep(timeSpan);
                Console.WriteLine("Prosla su 24 sata");

                CrawlerController cc = CrawlerController.getSingleton();
                if (cc != null)
                {
                    try
                    {
                        cc.crawlZara();
                        cc.crawlTerranova();
                        cc.crawlPullAndBear();
                    }
                    catch (Exception) { }
                }


            }
        }
    }
}