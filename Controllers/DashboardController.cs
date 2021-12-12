using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using prodotnet.Data;
using prodotnet.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;

namespace prodotnet.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }
        [TempData]
        public string StatusMessage {get;set;}
        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("idUser") != null)
            {
                ViewData["AuthorId"] = (string)HttpContext.Session.GetString("UserName");
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        //GET: Register

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        //POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                var check = _context.Users.FirstOrDefault(s => s.Username == user.Username || s.Email==user.Email);
                if (check == null)
                {
                    _context.Users.Add(user);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    StatusMessage = "Email hoặc tên đăng nhập đã tồn tại";
                    return RedirectToAction("Register");
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username,string password)
        {
            if (ModelState.IsValid)
            {
                var f_password = password;
                var data =_context.Users.Where(s => s.Username.Equals(username) && s.Password.Equals(f_password)).ToList();
                if (data.Count() > 0)
                {
                    var uername = data.FirstOrDefault().Username;
                    var mail = data.FirstOrDefault().Email;
                    var idUser = data.FirstOrDefault().IdUser;

                    HttpContext.Session.SetString("UserName",uername);
                    HttpContext.Session.SetString("Email",mail);
                    HttpContext.Session.SetInt32("idUser",idUser);
                    
                    return RedirectToAction(nameof(DashboardController.Index));
                }
                else
                {
                    ViewBag.error = "Lỗi đăng nhập";
                    StatusMessage = "Tài khoản bị mật khẩu hoặc email đăng nhập không đúng";
                    return RedirectToAction("Login");
                }
            }
            return View();
        }

        public ActionResult Logout()
        {
            HttpContext.Session.Clear();//remove session
            return RedirectToAction("Login");
        }
    }
}