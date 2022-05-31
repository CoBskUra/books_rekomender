using System;
using System.Collections.Generic;

namespace BooksRecommender.Messages.Shared
{
    public class MsgReadBook
    {
        public MsgReadBook(Models.ReadBook readBook)
        {
            var book = readBook.Book;
            id  = book.Id;
            title = book.Title;
            authors = book.Authors.Split(';');
            publisher = book.Publisher;
            genres = book.Genres.Split(';');
            targetGroups = book.TargetGroups.Split(';');
            tags= book.Tags.Split(';');
            numPages = book.NumPages;
            languageCode = book.LanguageCode;
            country = book.Country;
            publicationDate = book.PublicationDate;
            avgRating= book.AvgRating;
            ratingsCount = book.RatingsCount;
            monthRentals= book.MonthRentals;
            isFavourite = readBook.IsFavourite;
            rating = readBook.Rating;
        }
        public int id { get; set; }
        public string title { get; set; }
        public string[] authors { get; set; }
        public string publisher { get; set; }
        public string[] genres { get; set; }
        public string[] targetGroups { get; set; }
        public string[] tags { get; set; }
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
