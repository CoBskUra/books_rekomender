using System.Collections.Generic;

namespace BooksRecommender.Messages.Requests
{
    public class ShowBooksRequest
    {
        public string title;
        public string author;
        public List<string> genres;
        public List<string> tags;
        public double minRating;
        public double maxRating;
    }
}
