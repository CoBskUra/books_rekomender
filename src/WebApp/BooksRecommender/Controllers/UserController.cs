using BooksRecommender.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BooksRecommender.Messages.Requests;
using BooksRecommender.Messages.Responses;


using BooksRecommender.Services;
namespace BooksRecommender.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        public UserController(IUserService service) // przekazanie dbcontext
        {
            _userService = service;
        }

        [HttpPost]
        [Route("filter")]
        public async Task<IActionResult> ShowBooks([FromBody] ShowBooksRequest request)
        {
            try
            {
                var response  = await _userService.GetFilteredBooks(request);
                return Ok(response);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return BadRequest("Something went wrong");
            }
        }
 

        [HttpGet]
        [Route("{bookId}")]
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
        [Route("read/{email}")]
        public async Task<IActionResult> ShowReadBooks([FromRoute] string email)
        {
            List<Book> books = new List<Book>();

            try
            {
                var response = await _userService.GetUsersReadBooks(email);

                return Ok(response); // 0 książek to nie błąd, po prostu żadnej nie przeczytał
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return BadRequest("Something went wrong");
            }
        }


        [HttpPost]
        [Route("rate/{email}/{bookId}")]
        public async Task<IActionResult> SetRating([FromRoute]string email, [FromRoute]int bookId, [FromBody] SetRatingRequest request)
        {
            bool done;
            try
            {
                done = await _userService.SetBookAsRead(email, bookId, request.rating);
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
        [Route("read/{email}/{bookId}")]
        public async Task<IActionResult> SetAsRead([FromRoute]string email, [FromRoute] int bookId)
        {
            bool done;
            try
            {
                done = await _userService.UpdateUsersReadList(email, bookId);
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

        [HttpPost]
        [Route("favourite/{email}/{bookId}")]
        public async Task<IActionResult> SetAsFavourite([FromRoute] string email, [FromRoute] int bookId)
        {
            bool done;
            try
            {
                done = await _userService.SetBookAsFavourite(email, bookId);
                if (done)
                    return Ok("Saved");
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
        [Route("unfavourite/{email}/{bookId}")]
        public async Task<IActionResult> UnsetAsFavourite([FromRoute] string email, [FromRoute] int bookId)
        {
            bool done;
            try
            {
                done = await _userService.UnsetBookAsFavourite(email, bookId);
                if (done)
                    return Ok("Saved");
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
        [Route("recommend/favorites/{email}")]
        public async Task<IActionResult> GetRecommendationBasedOnFavorites([FromRoute]string email)
        {
            List<Book> books = new List<Book>();
            try
            {
                books = await _userService.RecommendFavorites(email);
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
        [Route("recommend/average/{email}")]
        public async Task<IActionResult> GetRecommendationBasedOnAverage([FromRoute]string email)
        {
            List<Book> books = new List<Book>();
            try
            {
                books = await _userService.RecommendAverage(email);
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
        [Route("recommend/basedOnBook/{email}/{bookId}")]
        public async Task<IActionResult> GetRecommendationBasedOnBook([FromRoute]string email, [FromRoute]int bookId)
        {
            List<Book> books = new List<Book>();
            try
            {
                books = await _userService.RecommendBasedOnBook(email, bookId);
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
