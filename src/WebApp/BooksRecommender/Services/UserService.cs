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
using System.Globalization;

namespace BooksRecommender.Services
{
    public interface IUserService
    {
        public Task<GetFilteredBooksResponse> GetFilteredBooks(ShowBooksRequest request);
        public Task<Book> GetBookDetails(int bId);
        public Task<GetUserReadBooksResponse> GetUsersReadBooks(string uId);
        public Task<bool> SetBookAsRead(string email, int bId, double? rating);
        public Task<bool> UpdateUsersReadList(string emailuId, int bId);
        public Task<List<MsgDisplayFilteredBook>> RecommendFavorites(string email);
        public Task<List<MsgDisplayFilteredBook>> RecommendAverage(string email);
        public Task<List<MsgDisplayFilteredBook>> RecommendBasedOnBook(string uId, int bId);
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
        public async Task<List<MsgDisplayFilteredBook>> RecommendFavorites(string email)
        {
            var favoriteBooks = _context.ReadBooks.Include("User").Include("Book").Where(c => c.User.Email == email && c.IsFavourite == true).ToList(); // serio na podstawie ulubionych
            var readBooks = _context.ReadBooks.Include("User").Include("Book").Where(c => c.User.Email == email).ToList();
            if (favoriteBooks == null || favoriteBooks.Count == 0)
                favoriteBooks = readBooks.OrderBy(b => b.Rating).Reverse().ToList().GetRange(0, 10);
            List<Book> books = new List<Book>();
            foreach (var fb in favoriteBooks)
                books.Add(fb.Book);
            return Recommend(books, email);
        }
        public async Task<List<MsgDisplayFilteredBook>> RecommendAverage(string email)
        {
            Book avgBook = new Book();

            var readBooks = _context.ReadBooks.Include("User").Include("Book").Where(c => c.User.Email == email).ToList();
            List<Book> books = new List<Book>();
            int i = (readBooks.Count >= 10) ? 10 : readBooks.Count;
            readBooks = readBooks.OrderBy(b => b.Rating).Reverse().ToList().GetRange(0, i);
            foreach (var b in readBooks)
            {
                books.Add(b.Book);
                /*start.ArgumentList.Add((b.Book.Id).ToString(CultureInfo.InvariantCulture));
                start.ArgumentList.Add(b.Book.Title);
                start.ArgumentList.Add(b.Book.Authors);
                start.ArgumentList.Add(b.Book.Publisher);
                start.ArgumentList.Add(b.Book.Genres);
                start.ArgumentList.Add(b.Book.TargetGroups);
                start.ArgumentList.Add(b.Book.Tags);
                start.ArgumentList.Add((b.Book.NumPages).ToString(CultureInfo.InvariantCulture));
                start.ArgumentList.Add(b.Book.LanguageCode);
                start.ArgumentList.Add(b.Book.Country);
                start.ArgumentList.Add((b.Book.PublicationDate.Year).ToString(CultureInfo.InvariantCulture));
                start.ArgumentList.Add((b.Book.AvgRating).ToString(CultureInfo.InvariantCulture));
                start.ArgumentList.Add((b.Book.RatingsCount).ToString(CultureInfo.InvariantCulture));
                start.ArgumentList.Add((b.Book.MonthRentals).ToString(CultureInfo.InvariantCulture));*/
            }

            avgBook = GetAverageBook(books);
            return Recommend(new List<Book> { avgBook }, email);
        }
        public async Task<List<MsgDisplayFilteredBook>> RecommendBasedOnBook(string email, int bId)
        {
            Book bk = _context.Books.Where(b => b.CsvId == bId).First();
            List<Book> bkInList = new List<Book> { bk };
            return Recommend(bkInList, email);
        }


        private List<MsgDisplayFilteredBook> Recommend(List<Book> list, string email)
        {
            List<MsgDisplayFilteredBook> recommendations = new List<MsgDisplayFilteredBook>();
            //string exeFile = "C:\\Users\\Zosia\\AppData\\Local\\Microsoft\\WindowsApps\\python3.10.exe";
            string exeFile = "C:\\Python310\\python.exe";
            string pyFile = "E:\\Reposy\\books_rekomender\\src\\WebApp\\BooksRecommender\\PythonCode\\Recomender.py";

            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = exeFile;
            start.ArgumentList.Add(pyFile);

            var readBooks = _context.ReadBooks.Include("User").Include("Book").Where(c => c.User.Email == email).ToList();

            foreach (var b in list)
            {
                start.ArgumentList.Add((b.Id).ToString(CultureInfo.InvariantCulture));
                start.ArgumentList.Add(b.Title);
                start.ArgumentList.Add(b.Authors);
                start.ArgumentList.Add(b.Publisher);
                start.ArgumentList.Add(b.Genres);
                start.ArgumentList.Add(b.TargetGroups);
                start.ArgumentList.Add(b.Tags);
                start.ArgumentList.Add((b.NumPages).ToString(CultureInfo.InvariantCulture));
                start.ArgumentList.Add(b.LanguageCode);
                start.ArgumentList.Add(b.Country);
                start.ArgumentList.Add((b.PublicationDate.Year).ToString(CultureInfo.InvariantCulture));
                start.ArgumentList.Add((b.AvgRating).ToString(CultureInfo.InvariantCulture));
                start.ArgumentList.Add((b.RatingsCount).ToString(CultureInfo.InvariantCulture));
                start.ArgumentList.Add((b.MonthRentals).ToString(CultureInfo.InvariantCulture));
            }

            start.UseShellExecute = false;
            start.CreateNoWindow = false;
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    var listAll = ReadCsvFile();

                    Console.WriteLine(result);

                    var x = AddSimilaritiesToBooks(result, listAll);
                    x = x.OrderBy(x => x.similarity).ToList();
                    x.Reverse();
                    foreach (var y in x)
                    {
                        bool read = false;
                        foreach (var r in readBooks)
                        {
                            if (r.Book.Id == y.bk.Id)
                            {
                                read = true;
                                break;
                            }
                        }
                        if (!read)
                        {
                            var book = y.bk;
                            var tmpbook = new Messages.Shared.MsgDisplayFilteredBook(book);
                            tmpbook.readByUser = _context.ReadBooks.Where(c => c.Book.Id == book.Id).Where(c => c.User.Email == email).Count() > 0;

                            recommendations.Add(tmpbook);
                            if (recommendations.Count >= 5)
                                break;
                        }
                    }
                }
            }
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
            bool oneAuthor = (str.Contains(",")) ? false : true;
            if (!oneAuthor)
            {
                if (str.Contains("\""))
                    str = str.Substring(2);
                else
                    str = str.Substring(1);
            }// bez "[
            else
            {
                str = str.Substring(str.IndexOf("\'") + 1);
                result.Add(str.Substring(0, str.IndexOf("\'")));
                return result.ToArray();
            }
            while (str.Length != 0)
            {
                string tmp;
                c = str.IndexOf('\'');
                if (c != -1) // jest element
                {
                    str = str.Substring(c + 1); // bez otwierającego '
                    c = str.IndexOf('\''); // zamykający '
                    tmp = str.Substring(0, c);
                    result.Add(tmp);
                    str = str.Substring(c + 2);// chyba?
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
            using (var reader = new StreamReader(@"E:\Reposy\books_rekomender\src\WebApp\BooksRecommender\PythonCode\books.csv"))
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
                        try
                        {
                            newBook.Id = Int32.Parse(values[0], CultureInfo.InvariantCulture);
                            newBook.Title = values[1];
                            newBook.Authors = values[2];
                            newBook.Publisher = values[3];
                            newBook.Genres = values[4];
                            newBook.TargetGroups = values[5];
                            newBook.Tags = values[6];
                            newBook.NumPages = Int32.Parse(values[7], CultureInfo.InvariantCulture);
                            newBook.LanguageCode = values[8];
                            newBook.Country = values[9];
                            newBook.PublicationDate = new DateTime(Int32.Parse(values[10], CultureInfo.InvariantCulture), 10, 10);
                            newBook.AvgRating = Double.Parse(values[11], CultureInfo.InvariantCulture);
                            newBook.RatingsCount = Int32.Parse(values[12], CultureInfo.InvariantCulture);
                            newBook.MonthRentals = Int32.Parse(values[13], CultureInfo.InvariantCulture);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        
                    }
                    else
                    {
                        int i = 0;
                        newBook.Id = Int32.Parse(values[0], CultureInfo.InvariantCulture);
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
                        newBook.NumPages = Int32.Parse(values[i], CultureInfo.InvariantCulture);
                        i++;
                        newBook.LanguageCode = values[i];
                        i++;
                        newBook.Country = values[i];
                        i++;
                        newBook.PublicationDate = new DateTime(Int32.Parse(values[i], CultureInfo.InvariantCulture), 10, 10);
                        i++;
                        newBook.AvgRating = Double.Parse(values[i], CultureInfo.InvariantCulture);
                        i++;
                        newBook.RatingsCount = Int32.Parse(values[i], CultureInfo.InvariantCulture);
                        i++;
                        newBook.MonthRentals = Int32.Parse(values[i], CultureInfo.InvariantCulture);
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
                if (sims[i].Contains('['))
                    sims[i] = sims[i].Substring(1);
                if (sims[i].Contains(']'))
                    sims[i] = sims[i].Substring(0, sims[i].IndexOf(']'));

                result.Add(new BookSim
                {
                    bk = books[i],
                    similarity = Double.Parse(sims[i], CultureInfo.InvariantCulture)
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

        private Book GetAverageBook(List<Book> books)
        {
            Book result = new Book();

            // id i title nie są ważne
            result.Id = 0;
            result.CsvId = 0;
            result.Title = "Average Book";

            Dictionary<string, int> publishers = new Dictionary<string, int>();
            Dictionary<string, int> targets = new Dictionary<string, int>();
            Dictionary<string, int> lang = new Dictionary<string, int>();
            Dictionary<string, int> country = new Dictionary<string, int>();
            Dictionary<string, int> genres = new Dictionary<string, int>();
            Dictionary<string, int> authors = new Dictionary<string, int>();
            Dictionary<string, int> tags = new Dictionary<string, int>();
            int sumPages = 0;
            int sumYear = 0;
            double sumRating = 0;
            int sumRatingCount = 0;
            int sumRentals = 0;
            int n = books.Count();

            foreach (Book b in books)
            {
                List<string> tmp = new List<string>();
                tmp = OneToMultipleString(b.Genres).ToList();
                foreach (var x in tmp)
                {
                    if (genres.ContainsKey(x))
                        genres[x]++;
                    else
                        genres.Add(x, 1);
                }
                tmp = new List<string>();
                tmp = OneToMultipleString(b.Authors).ToList();
                foreach (var x in tmp)
                {
                    if (authors.ContainsKey(x))
                        authors[x]++;
                    else
                        authors.Add(x, 1);
                }
                tmp = new List<string>();
                tmp = OneToMultipleString(b.Tags).ToList();
                foreach (var x in tmp)
                {
                    if (tags.ContainsKey(x))
                        tags[x]++;
                    else
                        tags.Add(x, 1);
                }

                sumPages += b.NumPages;
                sumYear += b.PublicationDate.Year;
                sumRating += b.AvgRating;
                sumRatingCount += b.RatingsCount;
                sumRentals += b.MonthRentals;

                if (publishers.ContainsKey(b.Publisher))
                    publishers[b.Publisher]++;
                else
                    publishers.Add(b.Publisher, 1);

                if (targets.ContainsKey(b.TargetGroups))
                    targets[b.TargetGroups]++;
                else
                    targets.Add(b.TargetGroups, 1);

                if (lang.ContainsKey(b.LanguageCode))
                    lang[b.LanguageCode]++;
                else
                    lang.Add(b.LanguageCode, 1);

                if (country.ContainsKey(b.Country))
                    country[b.Country]++;
                else
                    country.Add(b.Country, 1);
            }

            result.Publisher = publishers.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
            result.TargetGroups = targets.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
            result.LanguageCode = lang.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
            result.Country = country.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;

            result.NumPages = sumPages / n;
            result.MonthRentals = sumRentals / n;
            result.AvgRating = sumRating / n;
            result.PublicationDate = new DateTime(sumYear / n, 10, 10);
            result.RatingsCount = sumRatingCount / n;

            var list = authors.ToList();
            List<string> listSingle = new List<string>();
            list.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
            foreach (var v in list)
                listSingle.Add(v.Key);
            int i = (listSingle.Count() >= 5) ? 5 : listSingle.Count();
            result.Authors = MultipleToOneString(listSingle.GetRange(0, i).ToArray());

            list = genres.ToList();
            listSingle = new List<string>();
            list.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
            foreach (var v in list)
                listSingle.Add(v.Key);
            i = (listSingle.Count() >= 5) ? 5 : listSingle.Count();
            result.Genres = MultipleToOneString(listSingle.GetRange(0, i).ToArray());

            list = tags.ToList();
            listSingle = new List<string>();
            list.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
            foreach (var v in list)
                listSingle.Add(v.Key);
            i = (listSingle.Count() >= 5) ? 5 : listSingle.Count();
            result.Tags = MultipleToOneString(listSingle.GetRange(0, i).ToArray());

            return result;
        }
        private Book StringToBook(string res)
        {
            Book result = new Book();

            // id
            var ind = res.IndexOf("bookID");
            res = res.Substring(ind + 6);
            int offset = res.TakeWhile(c => char.IsWhiteSpace(c)).Count();
            ind = res.IndexOf("\r\n");
            result.CsvId = Int32.Parse(res.Substring(offset, ind - offset), CultureInfo.InvariantCulture);
            result.Id = result.CsvId;

            // title
            ind = res.IndexOf("title");
            res = res.Substring(ind + 5);
            offset = res.TakeWhile(c => char.IsWhiteSpace(c)).Count();
            ind = res.IndexOf("\r\n");
            result.Title = res.Substring(offset, ind - offset);

            //authors
            ind = res.IndexOf("authors");
            res = res.Substring(ind + 7);
            offset = res.TakeWhile(c => char.IsWhiteSpace(c)).Count();
            ind = res.IndexOf("\r\n");
            result.Authors = res.Substring(offset, ind - offset);

            //publisher
            ind = res.IndexOf("publisher");
            res = res.Substring(ind + 9);
            offset = res.TakeWhile(c => char.IsWhiteSpace(c)).Count();
            ind = res.IndexOf("\r\n");
            result.Publisher = res.Substring(offset, ind - offset);

            //Genres
            ind = res.IndexOf("Genres");
            res = res.Substring(ind + 6);
            offset = res.TakeWhile(c => char.IsWhiteSpace(c)).Count();
            ind = res.IndexOf("\r\n");
            result.Genres = res.Substring(offset, ind - offset);

            //target_groups
            ind = res.IndexOf("target_groups");
            res = res.Substring(ind + 13);
            offset = res.TakeWhile(c => char.IsWhiteSpace(c)).Count();
            ind = res.IndexOf("\r\n");
            result.TargetGroups = res.Substring(offset, ind - offset);

            //tags
            ind = res.IndexOf("tags");
            res = res.Substring(ind + 4);
            offset = res.TakeWhile(c => char.IsWhiteSpace(c)).Count();
            ind = res.IndexOf("\r\n");
            result.Tags = res.Substring(offset, ind - offset);

            //num_pages
            ind = res.IndexOf("num_pages");
            res = res.Substring(ind + 9);
            offset = res.TakeWhile(c => char.IsWhiteSpace(c)).Count();
            ind = res.IndexOf("\r\n");
            //var tmp = res.Substring(offset, ind - offset);
            result.NumPages = (int)Double.Parse(res.Substring(offset, ind - offset), CultureInfo.InvariantCulture);

            //language_code
            ind = res.IndexOf("language_code");
            res = res.Substring(ind + 13);
            offset = res.TakeWhile(c => char.IsWhiteSpace(c)).Count();
            ind = res.IndexOf("\r\n");
            result.LanguageCode = res.Substring(offset, ind - offset);

            //country_of_origin
            ind = res.IndexOf("country_of_origin");
            res = res.Substring(ind + 17);
            offset = res.TakeWhile(c => char.IsWhiteSpace(c)).Count();
            ind = res.IndexOf("\r\n");
            result.Country = res.Substring(offset, ind - offset);

            //publication_date
            ind = res.IndexOf("publication_date");
            res = res.Substring(ind + 17);
            offset = res.TakeWhile(c => char.IsWhiteSpace(c)).Count();
            ind = res.IndexOf("\r\n");
            result.PublicationDate = new DateTime((int)Double.Parse(res.Substring(offset, ind - offset), CultureInfo.InvariantCulture), 10, 10);

            //average_rating
            ind = res.IndexOf("average_rating");
            res = res.Substring(ind + 14);
            offset = res.TakeWhile(c => char.IsWhiteSpace(c)).Count();
            ind = res.IndexOf("\r\n");
            result.AvgRating = Double.Parse(res.Substring(offset, ind - offset), CultureInfo.InvariantCulture);

            //ratings_count
            ind = res.IndexOf("ratings_count");
            res = res.Substring(ind + 13);
            offset = res.TakeWhile(c => char.IsWhiteSpace(c)).Count();
            ind = res.IndexOf("\r\n");
            result.RatingsCount = (int)Double.Parse(res.Substring(offset, ind - offset), CultureInfo.InvariantCulture);

            //month_rentals
            ind = res.IndexOf("month_rentals");
            res = res.Substring(ind + 13);
            offset = res.TakeWhile(c => char.IsWhiteSpace(c)).Count();
            ind = res.IndexOf("\r\n");
            result.MonthRentals = (int)Double.Parse(res.Substring(offset, ind - offset), CultureInfo.InvariantCulture);

            return result;
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
