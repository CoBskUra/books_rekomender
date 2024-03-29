﻿using System.Collections.Generic;

namespace BooksRecommender.Messages.Requests
{
    public class ShowBooksRequest
    {
        public string title { get; set; } = "";
        public string author { get; set; } = "";
        public string genre { get; set; } = "";
        public string tag { get; set; } = "";
        public double minRating { get; set; } = 0;
        public double maxRating { get; set; } = 5;
        public string orderBy { get; set; } = "Title";
        public bool orderAscending { get; set; }= false;
        public int pageNumber { get; set; } = 1;
        public int pageSize { get; set; }= 10;
        public string email { get; set; } = "";
    }
}
