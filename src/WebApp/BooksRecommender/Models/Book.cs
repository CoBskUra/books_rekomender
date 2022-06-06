using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksRecommender.Models
{
    //bookID,title,authors,publisher,Genres,target_groups,tags,num_pages,language_code,country_of_origin,publication_date,average_rating,ratings_count,month_rentals
    public class Book
    {
        public int Id { get; set; }
        public int CsvId { get; set; }
        public string Title { get; set; }
        public string Authors { get; set; }
        public string Publisher { get; set; }
        public string Genres { get; set; }
        public string TargetGroups { get; set; }
        public string Tags { get; set; }
        public int NumPages { get; set; }
        public string LanguageCode { get; set; }
        public string Country { get; set; }
        public DateTime PublicationDate { get; set; }
        public double AvgRating { get; set; }
        public int RatingsCount { get; set; }
        public int MonthRentals { get; set; }
    }
}
