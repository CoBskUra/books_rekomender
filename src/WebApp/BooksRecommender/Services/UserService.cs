using BooksRecommender.Data;
using BooksRecommender.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace BooksRecommender.Services
{
    public interface IUserService
    {
        public Task<List<Book>> GetFilteredBooks(string title, string author, List<string> genres, List<string> tags, double minRating, double maxRating);
        public Task<Book> GetBookDetails(int bId);
        public Task<List<Book>> GetUsersReadBooks(int uId);
        public Task<bool> UpdateBookRating(int uId, int bId, double rating);
        public Task<bool> UpdateUsersReadList(int uId, int bId);
        public Task<List<Book>> RecommendFavorites(int uId);
        public Task<List<Book>> RecommendAverage(int uId);
        public Task<List<Book>> RecommendBasedOnBook(int uId, int bId);


    }
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }
        public Task<List<Book>> GetFilteredBooks(string title, string author, List<string> genres, List<string> tags, double minRating, double maxRating)
        {
            // dbcontext, where, itd
            return null;
        }
        public Task<Book> GetBookDetails(int bId)
        {
            // pobiera książkę o tym id
            return null;
        }
        public Task<List<Book>> GetUsersReadBooks(int uId)
        {
            // pobiera książki które użytkownik przeczytał
            // w sumie fajnie by było jakby jeszcze były widoczne jego ratingi tych książek
            // można by zrobić dodatkową klasę BooksWithIndividualRating?
            return null;
        }
        public async Task<bool> UpdateBookRating(int uId, int bId, double rating)
        {
            // książce zmienić avgRating i ratingsCount
            // użytkownikowi zapisać ten rating
            // jeśli nie miał oznaczonej książki jako przeczytaną - ustawić jako przeczytaną
            return false;
        }
        public async Task<bool> UpdateUsersReadList(int uId, int bId)
        {
            // użytkownikowi zapisać daną książkę w przeczytanych
            return false;
        }
        public Task<List<Book>> RecommendFavorites(int uId)
        {
            // wywołanie odpowiedniego algorytmu pythonowego
            // używając zapewne IronPython (instalacja przez nuget packages)
            return null;
        }
        public Task<List<Book>> RecommendAverage(int uId)
        {
            // wywołanie odpowiedniego algorytmu pythonowego, innego niż w favorites
            // używając zapewne IronPython (instalacja przez nuget packages)
            return null;
        }
        public Task<List<Book>> RecommendBasedOnBook(int uId, int bId)
        {
            // wywołanie odpowiedniego algorytmu pythonowego
            // używając zapewne IronPython (instalacja przez nuget packages)
            return null;
        }
    }
}
