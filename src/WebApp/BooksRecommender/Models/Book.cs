using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksRecommender.Models
{
    //bookID,title,authors,publisher,Genres,target_groups,tags,num_pages,language_code,country_of_origin,publication_date,average_rating,ratings_count,month_rentals
    public class Book
    {
        public int id { get; set; }
        public string title { get; set; }
        public List<string> authors { get; set; }
        public string publisher { get; set; }
        public List<string> genres { get; set; }
        public List<string> targetGroups { get; set; }
        public List<string> tags { get; set; }
        public int numPages { get; set; }
        public string languageCode { get; set; }
        public string country { get; set; }
        public DateTime publicationDate { get; set; }
        public double avgRating { get; set; }
        public int ratingsCount { get; set; }
        public int monthRentals { get; set; }
    }
}
