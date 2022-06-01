using BooksRecommender.Messages.Shared;
using System.Collections.Generic;

namespace BooksRecommender.Messages.Responses
{
    public class GetUserReadBooksResponse
    {
        public int numberOfPages { get; set; } = 1;
        public List<MsgReadBook> Books { get; set; } = new();
        
    }
}
