using BooksRecommender.Data;
using BooksRecommender.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using BooksRecommender.Messages.Responses;
using BooksRecommender.Messages.Requests;
using BooksRecommender.Messages.Shared;
using Microsoft.EntityFrameworkCore;

namespace BooksRecommender.Services
{
    public interface IUserService
    {
        public Task<GetFilteredBooksResponse> GetFilteredBooks(ShowBooksRequest request);
        public Task<Book> GetBookDetails(int bId);
        public Task<GetUserReadBooksResponse> GetUsersReadBooks(string uId);
        public Task<bool> SetBookAsRead(string uId, int bId, double? rating);
        public Task<bool> UpdateUsersReadList(string uId, int bId);
        public Task<List<Book>> RecommendFavorites(string uId);
        public Task<List<Book>> RecommendAverage(string uId);
        public Task<List<Book>> RecommendBasedOnBook(string uId, int bId);


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
            var books =  _context.Books.Where(c => request.ShouldApply(c));
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
            response.books= await books.Skip((request.pageNumber - 1) * request.pageSize)
                    .Take(request.pageSize).ToListAsync();

            return response;
        }
        public async Task<Book> GetBookDetails(int bId)
        {
            // pobiera książkę o tym id
            return _context.Books.Where(c => c.Id == bId).First();
        }
        public async Task<GetUserReadBooksResponse> GetUsersReadBooks(string uId)
        {
            // pobiera książki które użytkownik przeczytał
            // w sumie fajnie by było jakby jeszcze były widoczne jego ratingi tych książek
            // można by zrobić dodatkową klasę BooksWithIndividualRating?
            var books = _context.ReadBooks.Where(c => c.User.Id == uId).ToList();
            GetUserReadBooksResponse response = new();
            foreach(var book in books)
            {
                response.Books.Add(new MsgReadBook(book));
            }
            return response;
        }
        public async Task<bool> SetBookAsRead(string uId, int bId, double? rating)
        {
            // książce zmienić avgRating i ratingsCount
            // użytkownikowi zapisać ten rating
            // jeśli nie miał oznaczonej książki jako przeczytaną - ustawić jako przeczytaną
            _context.ReadBooks.Add(new ReadBook
            {
                Book = _context.Books.Where(c => c.Id == bId).First(),
                Rating = rating,
                User = _context.Users.Where(c => c.Id == uId).First()
            });
            _context.SaveChanges();
            return false;
        }
        public async Task<bool> UpdateUsersReadList(string uId, int bId)
        {
            _context.ReadBooks.Add(new ReadBook
            {
                Book = _context.Books.Where(c => c.Id == bId).First(),
                Rating = null,
                User = _context.Users.Where(c => c.Id == uId).First()
            });
            _context.SaveChanges();
            return true;
        }
        public Task<List<Book>> RecommendFavorites(string uId)
        {
            // wywołanie odpowiedniego algorytmu pythonowego
            // używając zapewne IronPython (instalacja przez nuget packages)
            return null;
        }
        public Task<List<Book>> RecommendAverage(string uId)
        {
            // wywołanie odpowiedniego algorytmu pythonowego, innego niż w favorites
            // używając zapewne IronPython (instalacja przez nuget packages)
            return null;
        }
        public Task<List<Book>> RecommendBasedOnBook(string uId, int bId)
        {
            // wywołanie odpowiedniego algorytmu pythonowego
            // używając zapewne IronPython (instalacja przez nuget packages)
            return null;
        }
    }
}
