namespace API.Entitites
{
    public class Article
    {
        public int Id { get; set; }
        public string href { get; set; }
        public string name { get; set; }
        public string[] imgSrc { get; set; }
        public string price { get; set; }
        public char gender { get; set; }
        public string type { get; set; }
    }
}