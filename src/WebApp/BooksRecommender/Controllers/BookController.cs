using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BooksRecommender.Data;

namespace BooksRecommender.Controllers
{
    [Route("api/seeder")]
    public class BookController : ControllerBase
    {
        // w sumie to nwm czy tu coś powinno być
        ApplicationDbContext _context;
        public BookController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        [Route("seed")]
        public void SeedDatabase([FromQuery]string path)
        {
            try
            {
                var seeder = new Seeder(_context);
                seeder.ReadFile(path);
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("Sucess");
        }
    }
}
