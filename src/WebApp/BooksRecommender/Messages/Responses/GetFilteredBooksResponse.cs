using System.Collections.Generic;

namespace BooksRecommender.Messages.Responses
{
    public class GetFilteredBooksResponse
    {
        public int numberOfPages { get; set; }
        public List<Models.Book> books { get; set; }
    }
}
