using CsvHelper;
using System.Globalization;
using System.IO;
using BooksRecommender.Models;
using System;

namespace BooksRecommender.Data
{
    public class Seeder
    {
        ApplicationDbContext _context;
        public Seeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public void ReadFile(string path)
        {
            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<BookInCsv>();
                foreach (var record in records)
                {
                    _context.Add(new Book
                    {
                        CsvId = Int32.Parse(record.bookID),
                        Title = record.title,
                        Authors = record.authors,
                        Publisher = record.publisher,
                        Genres = record.Genres,
                        TargetGroups = record.target_groups,
                        Tags = record.tags,
                        NumPages = Int32.Parse(record.num_pages),
                        LanguageCode = record.language_code,
                        Country = record.country_of_origin,
                        PublicationDate = new DateTime(Int32.Parse(record.publication_date),1,1),
                        AvgRating = double.Parse(record.average_rating, CultureInfo.InvariantCulture),
                        RatingsCount = Int32.Parse(record.ratings_count),
                        MonthRentals = Int32.Parse(record.month_rentals)
                    });
                }
                _context.SaveChanges();
            }
            
        }

        public class BookInCsv
        {
            public string bookID { get; set; }
            public string title{ get; set; }
            public string authors{ get; set; }
            public string publisher{ get; set; }
            public string Genres{ get; set; }
            public string target_groups{ get; set; }
            public string tags{ get; set; }
            public string num_pages{ get; set; }
            public string language_code{ get; set; }
            public string country_of_origin{ get; set; }
            public string publication_date{ get; set; }
            public string average_rating{ get; set; }
            public string ratings_count{ get; set; }
            public string month_rentals{ get; set; }

        }

    }
}
