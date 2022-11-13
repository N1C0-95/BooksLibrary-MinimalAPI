namespace MinimalApi.Models
{
    public class Book
    {
        public string Isbn { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Author { get; set; } = default!;
        public string ShortDescription { get; set; } = default!;
        public int PageCounter { get; set; }
        public DateTime ReleasedDate { get; set; }
    }
}
