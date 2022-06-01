using BooksRecommender.Data;
using BooksRecommender.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using BooksRecommender.Messages.Responses;
using BooksRecommender.Messages.Requests;
using BooksRecommender.Messages.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Analysis;
using System;
using IronPython.Hosting;
using System.IO;

namespace BooksRecommender.Services
{
    public interface IUserService
    {
        public Task<GetFilteredBooksResponse> GetFilteredBooks(ShowBooksRequest request);
        public Task<Book> GetBookDetails(int bId);
        public Task<GetUserReadBooksResponse> GetUsersReadBooks(string uId);
        public Task<bool> SetBookAsRead(string email, int bId, double? rating);
        public Task<bool> UpdateUsersReadList(string emailuId, int bId);
        public Task<List<Book>> RecommendFavorites(string email);
        public Task<List<Book>> RecommendAverage(string email);
        public Task<List<Book>> RecommendBasedOnBook(string uId, int bId);
        public Task<bool> SetBookAsFavourite(string email, int bId);


    }
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<GetFilteredBooksResponse> GetFilteredBooks(ShowBooksRequest request)
        {
            var books = _context.Books
                .Where(c => c.Title.Contains(request.title))
                .Where(c => c.Authors.Contains(request.author))
                .Where(c => c.AvgRating > request.minRating)
                .Where(c => c.AvgRating < request.maxRating)
                .Where(c => c.Tags.Contains(request.tag))
                .Where(c => c.Genres.Contains(request.genre));
            switch (request.orderBy)
            {
                case "Rating":
                    {
                        if (request.orderAscending) books = books.OrderBy(c => c.AvgRating);
                        else books = books.OrderByDescending(c => c.AvgRating);
                        break;
                    }
                default:
                    {
                        if(request.orderAscending) books = books.OrderBy(c => c.Title);
                        else books = books.OrderByDescending(c => c.Title);
                        break;
                    }
            }
            GetFilteredBooksResponse response = new();
            int count = books.Count();
            response.numberOfPages= (count)/request.pageSize;

            if (response.numberOfPages * request.pageSize < count) response.numberOfPages += 1;
            var books2 = await books.Skip((request.pageNumber - 1) * request.pageSize)
                    .Take(request.pageSize).ToListAsync();

            foreach(var book in books2)
            {
                var tmpbook = new Messages.Shared.MsgDisplayFilteredBook(book);
                tmpbook.readByUser = _context.ReadBooks.Where(c => c.Book.Id == book.Id).Where(c => c.User.Email == request.email).Count() > 0;
                response.books.Add(tmpbook);
            }
            return response;
        }
        public async Task<Book> GetBookDetails(int bId)
        {
            // pobiera książkę o tym id
            return _context.Books.Where(c => c.Id == bId).First();
        }
        public async Task<GetUserReadBooksResponse> GetUsersReadBooks(string email)
        {
            // pobiera książki które użytkownik przeczytał
            // w sumie fajnie by było jakby jeszcze były widoczne jego ratingi tych książek
            // można by zrobić dodatkową klasę BooksWithIndividualRating?
            var books = _context.ReadBooks.Include("User").Include("Book").Where(c => c.User.Email == email);
            GetUserReadBooksResponse response = new();
            foreach(var book in books)
            {
                response.Books.Add(new MsgReadBook(book));
            }
            return response;
        }
        public async Task<bool> SetBookAsRead(string email, int bId, double? rating)
        {
            // książce zmienić avgRating i ratingsCount
            // użytkownikowi zapisać ten rating
            // jeśli nie miał oznaczonej książki jako przeczytaną - ustawić jako przeczytaną
            _context.ReadBooks.Add(new ReadBook
            {
                Book = _context.Books.Where(c => c.Id == bId).First(),
                Rating = rating,
                User = _context.Users.Where(c => c.Email == email).First()
            });
            _context.SaveChanges();
            return false;
        }
        public async Task<bool> UpdateUsersReadList(string email, int bId)
        {
            _context.ReadBooks.Add(new ReadBook
            {
                Book = _context.Books.Where(c => c.Id == bId).First(),
                Rating = null,
                User = _context.Users.Where(c => c.Email == email).First()
            });
            _context.SaveChanges();
            return true;
        }
        public async Task<List<Book>> RecommendFavorites(string email)
        {
            // wywołanie odpowiedniego algorytmu pythonowego
            // używając zapewne IronPython (instalacja przez nuget packages)

            var engine = Python.CreateEngine();
            var source = engine.CreateScriptSourceFromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PythonCode\\Recomender.py"));
            var scope = engine.CreateScope();
            source.Execute(scope);
            var classRecommending = scope.GetVariable("recommending");
            var recommenderInstance = engine.Operations.CreateInstance(classRecommending);

            List<MsgReadBook> favoriteBooks = await GetFavoriteBooks(email);
            DataFrame bookDf = BooksToDF(favoriteBooks);
            DataFrame df = recommenderInstance.Recommend(bookDf);
            List<Book> recommendations = DFToBooks(df);

            return recommendations;
        }
        public async Task<List<Book>> RecommendAverage(string email)
        {
            // wywołanie odpowiedniego algorytmu pythonowego, innego niż w favorites
            // używając zapewne IronPython (instalacja przez nuget packages)

            var engine = Python.CreateEngine();
            var source = engine.CreateScriptSourceFromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PythonCode\\Recomender.py"));
            var scope = engine.CreateScope();
            source.Execute(scope);
            var classRecommending = scope.GetVariable("recommending");
            var recommenderInstance = engine.Operations.CreateInstance(classRecommending);

            var engine2 = Python.CreateEngine(); //potrzebne?
            var source2 = engine2.CreateScriptSourceFromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PythonCode\\Average_Book.py"));
            var scope2 = engine2.CreateScope();
            source2.Execute(scope2);
            var classAvgBook = scope.GetVariable("averageBook");
            var avgBookInstance = engine2.Operations.CreateInstance(classAvgBook);

            List<MsgReadBook> readBooks = (await GetUsersReadBooks(email)).Books;
            DataFrame df = BooksToDF(readBooks);
            DataFrame result = avgBookInstance.avr_book(df);
            List<Book> avgBook = DFToBooks(result);
            Book book = avgBook.First(); // powinna być i tak tylko jedna pozycja
            ReadBook readBook = new ReadBook 
            {
                Book = book
            };
            MsgReadBook bk = new MsgReadBook(readBook);
            List<MsgReadBook> books = new List<MsgReadBook> { bk };
            DataFrame bookDf = BooksToDF(books);
            DataFrame df2 = recommenderInstance.Recommend(bookDf);
            List<Book> recommendations = DFToBooks(df2);

            return recommendations;
        }
        public async Task<List<Book>> RecommendBasedOnBook(string email, int bId)
        {
            // wywołanie odpowiedniego algorytmu pythonowego
            // używając zapewne IronPython (instalacja przez nuget packages)
            var engine = Python.CreateEngine();
            var source = engine.CreateScriptSourceFromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PythonCode\\Recomender.py"));
            var scope = engine.CreateScope();
            source.Execute(scope);
            var classRecommending = scope.GetVariable("recommending");
            var recommenderInstance = engine.Operations.CreateInstance(classRecommending);

            Book book = await _context.Books.Where(b => b.Id == bId).FirstAsync();
            MsgReadBook bk = new MsgReadBook(new ReadBook { Book = book});
            List<MsgReadBook> books = new List<MsgReadBook> { bk };
            DataFrame bookDf = BooksToDF(books);
            DataFrame df = recommenderInstance.Recommend(bookDf);
            List<Book> recommendations = DFToBooks(df);

            return recommendations;
        }

        private string MultipleToOneString(string[] list)
        {
            string result = "\"[";
            for (int i = 0; i < list.Length; i++)
            {
                result.Concat("'");
                result.Concat(list[i]);
                result.Concat("'");
                if (i != list.Length - 1)
                    result.Concat(", ");
                else
                    result.Concat("]\"");
            }
            return result;
        }

        private string[] OneToMultipleString(string str)
        {
            List<string> result = new List<string>();
            int c;
            str = str.Substring(2); // bez "[
            while (str.Length != 0)
            {
                string tmp;
                c = str.IndexOf('\'');
                if (c != -1) // jest element
                {
                    str = str.Substring(1); // bez otwierającego '
                    c = str.IndexOf('\''); // zamykający '
                    tmp = str.Substring(0, c);
                    result.Add(tmp);
                    str = str.Substring(c + 1);// chyba?
                }
                else // koniec
                {
                    str = ""; // żeby Length był 0
                }

            }

            return result.ToArray();
        }

        private DataFrame BooksToDF(List<MsgReadBook> books)
        {
            PrimitiveDataFrameColumn<int> bookID = new PrimitiveDataFrameColumn<int>("bookID");
            StringDataFrameColumn title = new StringDataFrameColumn("title");
            StringDataFrameColumn authors = new StringDataFrameColumn("authors");
            StringDataFrameColumn publisher = new StringDataFrameColumn("publisher");
            StringDataFrameColumn Genres = new StringDataFrameColumn("Genres");
            StringDataFrameColumn target_groups = new StringDataFrameColumn("target_groups");
            StringDataFrameColumn tags = new StringDataFrameColumn("tags");
            PrimitiveDataFrameColumn<int> num_pages = new PrimitiveDataFrameColumn<int>("num_pages");
            StringDataFrameColumn language_code = new StringDataFrameColumn("language_code");
            StringDataFrameColumn country_of_origin = new StringDataFrameColumn("country_of_origin");
            PrimitiveDataFrameColumn<DateTime> publication_date = new PrimitiveDataFrameColumn<DateTime>("publication_date");
            PrimitiveDataFrameColumn<double> average_rating = new PrimitiveDataFrameColumn<double>("average_rating");
            PrimitiveDataFrameColumn<int> ratings_count = new PrimitiveDataFrameColumn<int>("ratings_count");
            PrimitiveDataFrameColumn<int> month_rentals = new PrimitiveDataFrameColumn<int>("month_rentals");

            foreach (MsgReadBook bk in books)
            {
                bookID.Append(bk.id); // chyba powinno być csvid?
                title.Append(bk.title);
                authors.Append(MultipleToOneString(bk.authors));
                publisher.Append(bk.publisher);
                Genres.Append(MultipleToOneString(bk.genres));
                target_groups.Append(MultipleToOneString(bk.targetGroups));
                tags.Append(MultipleToOneString(bk.tags));
                num_pages.Append(bk.numPages);
                language_code.Append(bk.languageCode);
                country_of_origin.Append(bk.country);
                publication_date.Append(bk.publicationDate);
                average_rating.Append(bk.avgRating);
                ratings_count.Append(bk.ratingsCount);
                month_rentals.Append(bk.monthRentals);
            }

            DataFrame df = new DataFrame(bookID, title, authors, publisher, Genres, target_groups, tags, num_pages, language_code,
                country_of_origin, publication_date, average_rating, ratings_count, month_rentals);
            return df;
        }

        private List<Book> DFToBooks(DataFrame df)
        {
            List<Book> books = new List<Book>();

            for (int i = 0; i < df.Rows.Count; i++)
            {
                Book book = new Book
                {
                    CsvId = (int)df["bookId"][i], // chyba ten id?
                    Authors = (string)df["title"][i],
                    Title = (string)df["title"][i],
                    AvgRating = (double)df["average_rating"][i],
                    Country = (string)df["country_of_origin"][i],
                    Publisher = (string)df["publisher"][i],
                    Genres = (string)df["Genres"][i],
                    TargetGroups = (string)df["target_groups"][i],
                    Tags = (string)df["tags"][i],
                    NumPages = (int)df["num_pages"][i],
                    LanguageCode = (string)df["language_code"][i],
                    PublicationDate = (DateTime)df["publication_date"][i],
                    RatingsCount = (int)df["ratings_count"][i],
                    MonthRentals = (int)df["month_rentals"][i]
                };
                books.Add(book);
            }

            return books;
        }

        private async Task<List<MsgReadBook>> GetFavoriteBooks(string uId)
        {
            var booksResponse = await GetUsersReadBooks(uId);
            var books = booksResponse.Books;
            //List<ReadBook> books = await GetUsersReadBooks(uId);
            books = books.OrderBy(b => b.avgRating).ToList();
            int i = books.Count >= 10 ? 10 : books.Count;
            books = books.GetRange(0, i);
            return books;
        }

        public async Task<bool> SetBookAsFavourite(string email, int bId)
        {
            var book = _context.ReadBooks.Where(b => b.Id == bId).First();
            book.IsFavourite = true;
            _context.SaveChanges();
            return true;
        }
    }
}
