using System;
using System.Collections.Generic;

namespace BooksRecommender.Messages.Shared
{
    public class MsgReadBook
    {
        public MsgReadBook(Models.ReadBook readBook)
        {
            var book = readBook.Book;
            id  = book.id;
            title = book.title;
            authors = book.authors;
            publisher = book.publisher;
            genres = book.genres;
            targetGroups = book.targetGroups;
            tags= book.tags;
            numPages = book.numPages;
            languageCode = book.languageCode;
            country = book.country;
            publicationDate = book.publicationDate;
            avgRating= book.avgRating;
            ratingsCount = book.ratingsCount;
            monthRentals= book.monthRentals;
            isFavourite = readBook.IsFavourite;
            rating = readBook.Rating;
        }
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
        public bool isFavourite { get; set; }
        public double? rating { get; set; }

    }
}
