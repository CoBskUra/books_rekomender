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
using System.Diagnostics;

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
        public Task<bool> UnsetBookAsFavourite(string email, int bId);


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
                        if (request.orderAscending) books = books.OrderBy(c => c.Title);
                        else books = books.OrderByDescending(c => c.Title);
                        break;
                    }
            }
            GetFilteredBooksResponse response = new();
            int count = books.Count();
            response.numberOfPages = (count) / request.pageSize;

            if (response.numberOfPages * request.pageSize < count) response.numberOfPages += 1;
            var books2 = await books.Skip((request.pageNumber - 1) * request.pageSize)
                    .Take(request.pageSize).ToListAsync();

            foreach (var book in books2)
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
            foreach (var book in books)
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
            Book testBook = new Book
            {
                Id = 1,
                CsvId = 1,
                Authors = "Ala Makota",
                AvgRating = 5.0,
                Country = "Polska",
                Genres = "adventure",
                LanguageCode = "pl",
                MonthRentals = 10,
                NumPages = 120,
                PublicationDate = new DateTime(2000, 10, 10),
                Publisher = "wydawnictwo",
                RatingsCount = 1902,
                Tags = "fajne",
                TargetGroups = "my",
                Title = "Jezu nie wiem jaki tytul"
            };
            //return new List<Book> { testBook };
            List<Book> recommendations = new List<Book>();
            string exeFile = "C:\\Users\\48695\\AppData\\Local\\Programs\\Python\\Python310\\python.exe";
            string pyFile = "C:\\Users\\48695\\Source\\Repos\\books_rekomender\\src\\WebApp\\BooksRecommender\\PythonCode\\Recomender.py";


            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = exeFile;
            start.ArgumentList.Add(pyFile);

            List<MsgReadBook> favoriteBooks = await GetFavoriteBooks(email);
            List<MsgReadBook> readBooks = (await GetUsersReadBooks(email)).Books;
            //readBooks = readBooks.OrderBy(r => r.id).ToList();
            DataFrame bookDf = BooksToDF(favoriteBooks);

            foreach (var b in favoriteBooks)
            {
                start.ArgumentList.Add((b.id).ToString());
                start.ArgumentList.Add(b.title);
                start.ArgumentList.Add(MultipleToOneString(b.authors));
                start.ArgumentList.Add(b.publisher);
                start.ArgumentList.Add(MultipleToOneString(b.genres));
                start.ArgumentList.Add(b.targetGroups);
                start.ArgumentList.Add(MultipleToOneString(b.tags));
                start.ArgumentList.Add((b.numPages).ToString());
                start.ArgumentList.Add(b.languageCode);
                start.ArgumentList.Add(b.country);
                start.ArgumentList.Add((b.publicationDate.Year).ToString());
                start.ArgumentList.Add((b.avgRating).ToString());
                start.ArgumentList.Add((b.ratingsCount).ToString());
                start.ArgumentList.Add((b.monthRentals).ToString());
            }

            start.UseShellExecute = false;
            start.CreateNoWindow = true;
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;
            bool returnn = false;
            using (Process process = Process.Start(start))
            {
                //return new List<Book> { testBook };
                using (StreamReader reader = process.StandardOutput)
                {
                    //return new List<Book> { testBook };
                    string result = reader.ReadToEnd();
                    return new List<Book> { testBook };
                    if (result.Length != 0)
                        returnn = true;
                    var list = ReadCsvFile();
                    var x = AddSimilaritiesToBooks(result, list);

                    //Console.WriteLine(x.Count);
                    x = x.OrderBy(x => x.similarity).ToList();
                    x.Reverse();
                    foreach (var y in x)
                    {
                        bool read = false;
                        foreach (var r in readBooks)
                        {
                            if (r.id == y.bk.Id)
                            {
                                read = true;
                                break;
                            }
                        }
                        if (!read)
                        {
                            recommendations.Add(y.bk);
                            if (recommendations.Count >= 5)
                                break;
                        }
                    }
                }
            }

            
                /*Book testBook = new Book
                {
                    Id = 1,
                    CsvId = 1,
                    Authors = "Ala Makota",
                    AvgRating = 5.0,
                    Country = "Polska",
                    Genres = "adventure",
                    LanguageCode = "pl",
                    MonthRentals = 10,
                    NumPages = 120,
                    PublicationDate = new DateTime(2000, 10, 10),
                    Publisher = "wydawnictwo",
                    RatingsCount = 1902,
                    Tags = "fajne",
                    TargetGroups = "my",
                    Title = "Jezu nie wiem jaki tytul"
                };
                return new List<Book> { testBook };*/
                //List<Book> recommendations = new List<Book> { testBook };
            


            //return recommendations;
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
            MsgReadBook bk = new MsgReadBook(new ReadBook { Book = book });
            List<MsgReadBook> books = new List<MsgReadBook> { bk };
            DataFrame bookDf = BooksToDF(books);
            DataFrame df = recommenderInstance.Recommend(bookDf);
            List<Book> recommendations = DFToBooks(df);

            return recommendations;
        }

        private string MultipleToOneString(string[] list)
        {
            string result = "";
            if (list.Length == 1)
            {
                result = "['";
                result = result + list[0];
                result = result + "']";
            }
            else
            {
                result = "\"[";
                for (int i = 0; i < list.Length; i++)
                {
                    result = result + "'";
                    result = result + list[i];
                    result = result + "'";
                    if (i != list.Length - 1)
                        result = result + ", ";
                    else
                        result = result + "]\"";
                }
            }
            return result;
        }

        private string[] OneToMultipleString(string str)
        {
            List<string> result = new List<string>();
            int c;
            bool oneAuthor = (str[0] == '\"') ? false : true;
            if (!oneAuthor)
                str = str.Substring(2); // bez "[
            else
                str = str.Substring(1);
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
                    str = str.Substring(c + 3);// chyba?
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
                target_groups.Append(bk.targetGroups);
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

        private List<Book> ReadCsvFile()
        {
            List<Book> result = new List<Book>();
            bool firstLine = true;
            using (var reader = new StreamReader(@"C:\Users\48695\source\repos\python_c_test\python_c_test\PythonCode\books.csv"))
            {
                while (!reader.EndOfStream)
                {
                    Book newBook = new Book();
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    if (firstLine)
                    {
                        firstLine = false;
                        continue;
                    }

                    bool multiple = (values.Length > 14) ? true : false;

                    if (!multiple)
                    {
                        newBook.Id = Int32.Parse(values[0]);
                        newBook.Title = values[1];
                        newBook.Authors = values[2];
                        newBook.Publisher = values[3];
                        newBook.Genres = values[4];
                        newBook.TargetGroups = values[5];
                        newBook.Tags = values[6];
                        newBook.NumPages = Int32.Parse(values[7]);
                        newBook.LanguageCode = values[8];
                        newBook.Country = values[9];
                        newBook.PublicationDate = new DateTime(Int32.Parse(values[10]), 10, 10);
                        newBook.AvgRating = Double.Parse(values[11]);
                        newBook.RatingsCount = Int32.Parse(values[12]);
                        newBook.MonthRentals = Int32.Parse(values[13]);
                    }
                    else
                    {
                        int i = 0;
                        newBook.Id = Int32.Parse(values[0]);
                        newBook.Title = values[1];
                        if (values[2].Contains("\"[") && !values[2].Contains(']'))
                        {
                            string tmp = values[2];
                            for (i = 3; i < values.Length; i++)
                            {
                                tmp = tmp + ", " + values[i];
                                if (values[i].Contains(']'))
                                    break;
                            }
                            newBook.Authors = tmp;
                        }
                        else
                        {
                            newBook.Authors = values[2];
                            i = 2;
                        }
                        i++;
                        newBook.Publisher = values[i];
                        i++;
                        if (values[i].Contains("\"[") && !values[i].Contains(']'))
                        {
                            string tmp = values[i];
                            for (i = i + 1; i < values.Length; i++)
                            {
                                tmp = tmp + ", " + values[i];
                                if (values[i].Contains(']'))
                                    break;
                            }
                            newBook.Genres = tmp;
                        }
                        else
                        {
                            newBook.Genres = values[i];
                        }
                        i++;
                        newBook.TargetGroups = values[i];
                        i++;
                        if (values[i].Contains("\"[") && !values[i].Contains(']'))
                        {
                            string tmp = values[i];
                            for (i = i + 1; i < values.Length; i++)
                            {
                                tmp = tmp + ", " + values[i];
                                if (values[i].Contains(']'))
                                    break;
                            }
                            newBook.Tags = tmp;
                        }
                        else
                        {
                            newBook.Tags = values[i];
                        }
                        i++;
                        newBook.NumPages = Int32.Parse(values[i]);
                        i++;
                        newBook.LanguageCode = values[i];
                        i++;
                        newBook.Country = values[i];
                        i++;
                        newBook.PublicationDate = new DateTime(Int32.Parse(values[i]), 10, 10);
                        i++;
                        newBook.AvgRating = Double.Parse(values[i]);
                        i++;
                        newBook.RatingsCount = Int32.Parse(values[i]);
                        i++;
                        newBook.MonthRentals = Int32.Parse(values[i]);
                    }

                    result.Add(newBook);
                }
            }
            return result;
        }

        public List<BookSim> AddSimilaritiesToBooks(string sim, List<Book> books)
        {
            string[] sims = sim.Split(',');
            List<BookSim> result = new List<BookSim>();
            for (int i = 0; i < sims.Length; i++)
            {
                double s;
                if (sims[i].Contains('['))
                    sims[i] = sims[i].Substring(1);
                if (sims[i].Contains(']'))
                    sims[i] = sims[i].Substring(0, sims[i].IndexOf(']'));
                result.Add(new BookSim
                {
                    bk = books[i],
                    similarity = double.Parse(sims[i])
                });
            }
            return result;
        }
        private async Task<List<MsgReadBook>> GetFavoriteBooks(string email)
        {
            var booksResponse = await GetUsersReadBooks(email);
            var books = booksResponse.Books;
            //List<ReadBook> books = await GetUsersReadBooks(uId);
            books = books.OrderBy(b => b.avgRating).ToList();
            books.Reverse();
            int i = books.Count >= 10 ? 10 : books.Count;
            books = books.GetRange(0, i);
            return books;
        }

        public async Task<bool> SetBookAsFavourite(string email, int bId)
        {
            var book = _context.ReadBooks.Where(c => c.User.Email == email).Where(c => c.Book.Id == bId).First();
            book.IsFavourite = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnsetBookAsFavourite(string email, int bId)
        {
            var book = _context.ReadBooks.Where(c => c.User.Email == email).Where(c => c.Book.Id == bId).First();
            book.IsFavourite = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
