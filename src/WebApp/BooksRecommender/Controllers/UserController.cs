using BooksRecommender.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksRecommender.Controllers
{
    [ApiController]
    public class UserController : Controller
    {
        public UserController() // przekazanie dbcontext
        {
        }

        [HttpGet]
        [Route("books/filter")]
        public async Task<IActionResult> ShowBooks(string title, string author, List<string> genres, List<string> tags, double minRating, double maxRating)
        {
            List<Book> books = new List<Book>();

            try
            {
                books = await GetFilteredBooks(title, author, genres, tags, minRating, maxRating);
                if (books == null || books.Count == 0)
                    return NotFound("Data not found");
                return Ok(books);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return BadRequest("Something went wrong");
            }
        }
        public Task<List<Book>> GetFilteredBooks(string title, string author, List<string> genres, List<string> tags, double minRating, double maxRating)
        {
            // dbcontext, where, itd
            return null;
        }

        [HttpGet]
        [Route("books/{bookId}")]
        public async Task<IActionResult> GetBook([FromRoute]int bookId)
        {
            Book book;
            try
            {
                book = await GetBookDetails(bookId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return BadRequest("Something went wrong");
            }

            if (book == null)
                return NotFound("Data not found");
            return Ok(book);
        }
        public Task<Book> GetBookDetails(int bId)
        {
            // pobiera książkę o tym id
            return null;
        }

        [HttpGet]
        [Route("books/read/{userId}")]
        public async Task<IActionResult> ShowReadBooks([FromRoute] int userId)
        {
            List<Book> books = new List<Book>();

            try
            {
                books = await GetUsersReadBooks(userId);
                if (books == null)
                    return NotFound("Data not found");
                return Ok(books); // 0 książek to nie błąd, po prostu żadnej nie przeczytał
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return BadRequest("Something went wrong");
            }
        }
        public Task<List<Book>> GetUsersReadBooks(int uId)
        {
            // pobiera książki które użytkownik przeczytał
            // w sumie fajnie by było jakby jeszcze były widoczne jego ratingi tych książek
            // można by zrobić dodatkową klasę BooksWithIndividualRating?
            return null;
        }

        [HttpPost]
        [Route("books/rate/{userId}/{bookId}")]
        public async Task<IActionResult> SetRating([FromRoute]int userId, [FromRoute]int bookId, double rating)
        {
            bool done;
            try
            {
                done = await UpdateBookRating(userId, bookId, rating);
                if (done)
                    return Ok("Your rating has been saved");
                else 
                    return NotFound("Data not found");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return BadRequest("Something went wrong");
            }
        }
        public async Task<bool> UpdateBookRating(int uId, int bId, double rating)
        {
            // książce zmienić avgRating i ratingsCount
            // użytkownikowi zapisać ten rating
            // jeśli nie miał oznaczonej książki jako przeczytaną - ustawić jako przeczytaną
            return false;
        }

        [HttpPost]
        [Route("book/read/{userId}/{bookId}")]
        public async Task<IActionResult> SetAsRead([FromRoute]int userId, [FromRoute] int bookId)
        {
            bool done;
            try
            {
                done = await UpdateUsersReadList(userId, bookId);
                if (done)
                    return Ok("Your read books have been updated");
                else
                    return NotFound("Data not found");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return BadRequest("Something went wrong");
            }
        }
        public async Task<bool> UpdateUsersReadList(int uId, int bId)
        {
            // użytkownikowi zapisać daną książkę w przeczytanych
            return false;
        }

        [HttpGet]
        [Route("recommend/favorites/{userId}")]
        public async Task<IActionResult> GetRecommendationBasedOnFavorites([FromRoute]int userId)
        {
            List<Book> books = new List<Book>();
            try
            {
                books = await RecommendFavorites(userId);
                if (books == null || books.Count == 0)
                    return NotFound("Data not found"); // uznajemy że coś zawsze trzeba polecić
                else
                    return Ok(books);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return BadRequest("Something went wrong");
            }
        }
        public Task<List<Book>> RecommendFavorites(int uId)
        {
            // wywołanie odpowiedniego algorytmu pythonowego
            // używając zapewne IronPython (instalacja przez nuget packages)
            return null;
        }

        [HttpGet]
        [Route("recommend/average/{userId}")]
        public async Task<IActionResult> GetRecommendationBasedOnAverage([FromRoute]int userId)
        {
            List<Book> books = new List<Book>();
            try
            {
                books = await RecommendAverage(userId);
                if (books == null || books.Count == 0)
                    return NotFound("Data not found");
                else
                    return Ok(books);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return BadRequest("Something went wrong");
            }
        }
        public Task<List<Book>> RecommendAverage(int uId)
        {
            // wywołanie odpowiedniego algorytmu pythonowego, innego niż w favorites
            // używając zapewne IronPython (instalacja przez nuget packages)
            return null;
        }

        [HttpGet]
        [Route("recommend/basedOnBook/{userId}/{bookId}")]
        public async Task<IActionResult> GetRecommendationBasedOnBook([FromRoute]int userId, [FromRoute]int bookId)
        {
            List<Book> books = new List<Book>();
            try
            {
                books = await RecommendBasedOnBook(userId, bookId);
                if (books == null || books.Count == 0)
                    return NotFound("Data not found");
                else
                    return Ok(books);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return BadRequest("Something went wrong");
            }
        }
        public Task<List<Book>> RecommendBasedOnBook(int uId, int bId)
        {
            // wywołanie odpowiedniego algorytmu pythonowego
            // używając zapewne IronPython (instalacja przez nuget packages)
            return null;
        }       
    }
}
