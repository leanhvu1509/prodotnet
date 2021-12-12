using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using prodotnet.Data;
using prodotnet.Models;
using prodotnet.Models.ViewModel;

namespace prodotnet.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            HomeViewModel homeViewModel = new HomeViewModel();
            homeViewModel.ListPosts = _context.Posts.Include(p => p.User)
                        .Include(p => p.PostCategories)
                        .ThenInclude(c => c.Category)
                        .AsNoTracking().Where(q => Convert.ToInt32(q.Published)==1).OrderByDescending(s =>s.DateCreated).ToList();

            return View(homeViewModel);
        }

        [Route("{slug}.html", Name = "viewonepost")]
        public async Task<IActionResult> DisplayPost()
        {

            string Slug = (string)Request.RouteValues["slug"];

            if (string.IsNullOrEmpty(Slug))
            {
                return NotFound("Không thấy trang");
            }

            var post = await _context.Posts
                .Where(p => p.Slug == Slug)
                .Include(p => p.User)
                .Include(p => p.PostCategories)
                .ThenInclude(c => c.Category)
                .FirstOrDefaultAsync();

            if (post == null)
            {
                return NotFound("Không thấy trang");
            }
            return View(post);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
