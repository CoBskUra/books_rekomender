using BooksRecommender.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BooksRecommender.Messages.Requests;

using BooksRecommender.Services;
namespace BooksRecommender.Controllers
{
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        public UserController(IUserService service) // przekazanie dbcontext
        {
            _userService = service;
        }

        [HttpGet]
        [Route("books/filter")]
        public async Task<IActionResult> ShowBooks([FromBody] ShowBooksRequest request)
        {
            List<Book> books = new List<Book>();

            try
            {
                books = await _userService.GetFilteredBooks(request.title, request.author, request.genres, request.tags, request.minRating, request.maxRating);
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
 

        [HttpGet]
        [Route("books/{bookId}")]
        public async Task<IActionResult> GetBook([FromRoute]int bookId)
        {
            Book book;
            try
            {
                book = await _userService.GetBookDetails(bookId);
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


        [HttpGet]
        [Route("books/read/{userId}")]
        public async Task<IActionResult> ShowReadBooks([FromRoute] int userId)
        {
            List<Book> books = new List<Book>();

            try
            {
                books = await _userService.GetUsersReadBooks(userId);
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


        [HttpPost]
        [Route("books/rate/{userId}/{bookId}")]
        public async Task<IActionResult> SetRating([FromRoute]int userId, [FromRoute]int bookId, [FromBody] SetRatingRequest request)
        {
            bool done;
            try
            {
                done = await _userService.UpdateBookRating(userId, bookId, request.rating);
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


        [HttpPost]
        [Route("book/read/{userId}/{bookId}")]
        public async Task<IActionResult> SetAsRead([FromRoute]int userId, [FromRoute] int bookId)
        {
            bool done;
            try
            {
                done = await _userService.UpdateUsersReadList(userId, bookId);
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


        [HttpGet]
        [Route("recommend/favorites/{userId}")]
        public async Task<IActionResult> GetRecommendationBasedOnFavorites([FromRoute]int userId)
        {
            List<Book> books = new List<Book>();
            try
            {
                books = await _userService.RecommendFavorites(userId);
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


        [HttpGet]
        [Route("recommend/average/{userId}")]
        public async Task<IActionResult> GetRecommendationBasedOnAverage([FromRoute]int userId)
        {
            List<Book> books = new List<Book>();
            try
            {
                books = await _userService.RecommendAverage(userId);
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


        [HttpGet]
        [Route("recommend/basedOnBook/{userId}/{bookId}")]
        public async Task<IActionResult> GetRecommendationBasedOnBook([FromRoute]int userId, [FromRoute]int bookId)
        {
            List<Book> books = new List<Book>();
            try
            {
                books = await _userService.RecommendBasedOnBook(userId, bookId);
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
      
    }
}
