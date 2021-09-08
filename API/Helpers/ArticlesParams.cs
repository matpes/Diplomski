namespace API.Helpers
{
    public class ArticlesParams
    {
        private const int MaxPageSize = 108;
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 4;

        public int pageSize{
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize)? MaxPageSize : value;
        }

        public char Gender { get; set; }
        public string [] Categories { get; set; }

        public int Sort { get; set; }
    }
}