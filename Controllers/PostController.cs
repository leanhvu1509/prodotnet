using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using prodotnet.Data;
using prodotnet.Models;
using prodotnet.utils;

namespace prodotnet.Controllers
{
    public class PostController : Controller
    {
        private readonly AppDbContext _context;
        private IWebHostEnvironment _environment;
        public PostController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        [TempData]
        public string StatusMessage { get; set; }
        // GET: Post
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            ViewData["AuthorId"] = HttpContext.Session.GetString("UserName");
            var posts = _context.Posts.Include(p => p.User)
                        .Include(p => p.PostCategories)
                        .ThenInclude(c => c.Category)
                        .AsNoTracking();
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ?"name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            if (searchString != null) { 
                pageNumber = 1; } 
            else { 
                searchString = currentFilter; }

            ViewData["CurrentFilter"] = searchString;
            if (!String.IsNullOrEmpty(searchString)) 
            { 
                posts = posts.Where(s => s.Title.Contains(searchString) || s.Description.Contains(searchString)); 
            }

            switch (sortOrder)
            {
                case "name_desc":
                    posts = posts.OrderByDescending(s => s.Title);
                    break;
                case "Date":
                    posts = posts.OrderBy(s => s.DateUpdated);
                    break;
                case "date_desc":
                    posts = posts.OrderByDescending(s => s.DateUpdated);
                    break;
                default:
                    posts = posts.OrderByDescending(s => s.DateCreated);
                    break;
            }
            int pageSize = 4;
            // return View(await posts.ToListAsync());
            return View(await PaginatedList<Post>.CreateAsync(posts, pageNumber ?? 1, pageSize));
        }

        // GET: Post/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["AuthorId"] = HttpContext.Session.GetString("UserName");
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Post/Create
        public async Task<IActionResult> Create()
        {
            ViewData["AuthorId"] = HttpContext.Session.GetString("UserName");
            var categories = await _context.Categories.ToListAsync();
            ViewData["categories"] = new MultiSelectList(categories, "Id", "Title");
            return View();
        }

        // POST: Post/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Slug,Content,ImageFile,Published,CategoryIDs")] CreatePostModel post)
        {
            ViewData["AuthorId"] = HttpContext.Session.GetString("UserName");
            var categories = await _context.Categories.ToListAsync();
            ViewData["categories"] = new MultiSelectList(categories, "Id", "Title");

            if (await _context.Posts.AnyAsync(p => p.Slug == post.Slug))
            {
                ModelState.AddModelError("Slug", "Nhập chuỗi url khác");
                return View(post);
            }

            if (ModelState["Slug"].ValidationState == ModelValidationState.Invalid)
            {
                post.Slug = Utils.GenerateSlug(post.Title);
                ModelState.SetModelValue("Slug", new ValueProviderResult(post.Slug));
                // Thiết lập và kiểm tra lại Model
                ModelState.Clear();
                TryValidateModel(post);
            }

            if (ModelState.IsValid)
            {
                //Save image to wwwroot/image
                string wwwRootPath = _environment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(post.ImageFile.FileName);
                string extension = Path.GetExtension(post.ImageFile.FileName);
                post.ImageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                string path = Path.Combine(wwwRootPath + "/upload/", fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await post.ImageFile.CopyToAsync(fileStream);
                }

                var user = HttpContext.Session.GetInt32("idUser");
                post.DateCreated = post.DateUpdated = DateTime.Now;
                post.AuthorId = (int)user;

                _context.Add(post);
                if (post.CategoryIDs != null)
                {
                    foreach (var CateID in post.CategoryIDs)
                    {
                        _context.Add(new PostCategory()
                        {
                            CategoryID = CateID,
                            Post = post
                        });
                    }
                }
                await _context.SaveChangesAsync();
                StatusMessage = "Vừa tạo bài viết mới";
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "IdUser", "Username", post.AuthorId);
            return View(post);
        }

        // GET: Post/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["AuthorId"] = HttpContext.Session.GetString("UserName");
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.Include(p => p.PostCategories).FirstOrDefaultAsync(p => p.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            var postEdit = new CreatePostModel()
            {
                PostId = post.PostId,
                Title = post.Title,
                Content = post.Content,
                Description = post.Description,
                Slug = post.Slug,
                Published = post.Published,
                CategoryIDs = post.PostCategories.Select(pc => pc.CategoryID).ToArray()
            };

            var categories = await _context.Categories.ToListAsync();
            ViewData["categories"] = new MultiSelectList(categories, "Id", "Title");
            return View(postEdit);
        }

        // POST: Post/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,Title,Description,Slug,Content,Published,CategoryIDs")] CreatePostModel post)
        {
            ViewData["AuthorId"] = HttpContext.Session.GetString("UserName");
            var categories = await _context.Categories.ToListAsync();
            ViewData["categories"] = new MultiSelectList(categories, "Id", "Title");
            if (await _context.Posts.AnyAsync(p => p.Slug == post.Slug && p.PostId != id))
            {
                ModelState.AddModelError("Slug", "Nhập chuỗi url khác");
                return View(post);
            }

            if (ModelState["Slug"].ValidationState == ModelValidationState.Invalid)
            {
                post.Slug = Utils.GenerateSlug(post.Title);
                ModelState.SetModelValue("Slug", new ValueProviderResult(post.Slug));
                // Thiết lập và kiểm tra lại Model
                ModelState.Clear();
                TryValidateModel(post);
            }

            if (id != post.PostId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var postUpdate = await _context.Posts.Include(p => p.PostCategories).FirstOrDefaultAsync(p => p.PostId == id);
                    if (postUpdate == null)
                    {
                        return NotFound();
                    }
                    postUpdate.Title = post.Title;
                    postUpdate.Content = post.Content;
                    postUpdate.Description = post.Description;
                    postUpdate.Slug = post.Slug;
                    postUpdate.Published = post.Published;
                    postUpdate.DateUpdated = DateTime.Now;

                    if (post.CategoryIDs == null) post.CategoryIDs = new int[] { };
                    var oldCateIDs = postUpdate.PostCategories.Select(c => c.CategoryID).ToArray();
                    var newCateIDs = post.CategoryIDs;

                    var removeCatePosts = from postCate in postUpdate.PostCategories
                                          where (!newCateIDs.Contains(postCate.CategoryID))
                                          select postCate;
                    _context.PostCategories.RemoveRange(removeCatePosts);

                    var addCateIDs = from cateID in newCateIDs
                                     where (!oldCateIDs.Contains(cateID))
                                     select cateID;
                    foreach (var cateId in addCateIDs)
                    {
                        _context.PostCategories.Add(new PostCategory()
                        {
                            PostID = id,
                            CategoryID = cateId
                        });
                    }

                    _context.Update(postUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.PostId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                StatusMessage = "Vừa cập nhật bài viết thành công";
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "IdUser", "Username", post.AuthorId);
            return View(post);
        }

        // GET: Post/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewData["AuthorId"] = HttpContext.Session.GetString("UserName");
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Post/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ViewData["AuthorId"] = HttpContext.Session.GetString("UserName");
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            var imagePath = Path.Combine(_environment.WebRootPath, "upload", post.ImageName);
            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            StatusMessage = "Bạn vừa xóa bài viết: " + post.Title;
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }
    }
}
