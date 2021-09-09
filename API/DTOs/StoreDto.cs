using System;

namespace API.DTOs
{
    public class StoreDto
    {
        public int Id { get; set; }
        public string name { get; set; }
        public bool crawling {get; set;}
        public DateTime lastTimeCrawled { get; set; }
    }
}