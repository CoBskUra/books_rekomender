namespace BooksRecommender.Models
{
    public class ReadBook
    {
        public int Id { get; set; }
        public Book Book { get; set; }
        public ApplicationUser User { get; set; }
        public bool IsFavourite { get; set; }
        public double? Rating { get; set; }
    }
}
