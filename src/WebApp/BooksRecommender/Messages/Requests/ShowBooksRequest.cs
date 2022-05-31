using System.Collections.Generic;

namespace BooksRecommender.Messages.Requests
{
    public class ShowBooksRequest
    {
        public string title { get; set; }
        public string author { get; set; }
        public List<string> genres { get; set; }
        public List<string> tags { get; set; }
        public double minRating { get; set; }
        public double maxRating { get; set; }
        public string orderBy { get; set; } = "Title";
        public bool orderAscending { get; set; }= false;
        public int pageNumber { get; set; } = 1;
        public int pageSize { get; set; }= 10;

        public bool ShouldApply(Models.Book book)
        {
            if(!book.Title.Contains(title)) return false;

            if(book.AvgRating < minRating || book.AvgRating > maxRating) return false;

            if (!book.Authors.Contains(author)) return false;

            foreach(var genre in genres)if(! book.Genres.Contains(genre)) return false;

            foreach(var tags in tags) if(! book.Tags.Contains(tags)) return false;

            return true;
        }
    }
}
