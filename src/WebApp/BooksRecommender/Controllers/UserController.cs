using BooksRecommender.Services;
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
        private IUserSignInManager signInManager;
        private IDatabase dbManager;
        public UserController(IUserSignInManager signmg, IDatabase db)
        {
            signInManager = signmg;
            dbManager = db;
        }

        public void ShowBooks()
        {

        }
        public void SetRating()
        {

        }
        public void SetAsRead()
        {

        }
        public void GetRecommendationBasedOnFavorites()
        {

        }
        public void GetRecommendationBasedOnAverage()
        {

        }
        public void GetRecommendationBasedOnBook()
        {

        }
        public void ShowReadBooks()
        {

        }
    }
}
