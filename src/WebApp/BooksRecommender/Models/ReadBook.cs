namespace BooksRecommender.Models
{
    public class ReadBook
    {
        public int Id { get; set; }
        public virtual Book Book { get; set; }
        public virtual ApplicationUser User { get; set; }
        public bool IsFavourite { get; set; }
        public double? Rating { get; set; }
    }
}
