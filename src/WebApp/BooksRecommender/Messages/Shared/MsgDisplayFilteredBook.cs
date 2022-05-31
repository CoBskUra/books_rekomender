
using System;
using System.Collections.Generic;

namespace BooksRecommender.Messages.Shared
    {
        public class MsgDisplayFilteredBook
        {
            public MsgDisplayFilteredBook(Models.Book readBook)
            {
                var book = readBook;
                id = book.Id;
                title = book.Title;
                authors = book.Authors.Trim(new char[] { '[', ']', ' ' }).Split(',');
                publisher = book.Publisher;
                genres = book.Genres.Split(';');
                targetGroups = book.TargetGroups.Trim(new char[] { '[', ']', ' ' }).Split(',');
                tags = book.Tags.Trim(new char[] { '[', ']', ' ' }).Split(',');
                numPages = book.NumPages;
                languageCode = book.LanguageCode;
                country = book.Country;
                publicationDate = book.PublicationDate.ToString("yyyy");
                avgRating = book.AvgRating;
                ratingsCount = book.RatingsCount;
                monthRentals = book.MonthRentals;


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
            public string publicationDate { get; set; }
            public double avgRating { get; set; }
            public int ratingsCount { get; set; }
            public int monthRentals { get; set; }
            public bool readByUser { get; set; }
        }
    }
