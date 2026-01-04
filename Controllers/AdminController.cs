using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Neksara.Data;
using Neksara.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Neksara.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PAGE_SIZE = 5;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // DASHBOARD
        // =========================
        public IActionResult Index()
        {
            return RedirectToAction(nameof(AdminPanel));
        }

        public IActionResult AdminPanel()
        {
            return View();
        }

        // =========================
        // CATEGORY CRUD
        // =========================
        public async Task<IActionResult> CategoryIndex(string search, int page = 1)
        {
            var query = _context.Categories.Where(c => !c.IsDeleted);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(c => c.CategoryName.Contains(search));

            int totalData = await query.CountAsync();

            var data = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * PAGE_SIZE)
                .Take(PAGE_SIZE)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = Math.Ceiling(totalData / (double)PAGE_SIZE);
            ViewBag.Search = search;

            return View(data);
        }

        public IActionResult CreateCategory() => View();

        [HttpPost]
        public async Task<IActionResult> CreateCategory(Category model)
        {
            if (!ModelState.IsValid) return View(model);

            model.CreatedAt = DateTime.Now;
            model.IsDeleted = false;

            _context.Categories.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(CategoryIndex));
        }

        public async Task<IActionResult> EditCategory(int id)
        {
            var data = await _context.Categories.FindAsync(id);
            if (data == null) return NotFound();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> EditCategory(Category model)
        {
            if (!ModelState.IsValid) return View(model);

            model.UpdatedAt = DateTime.Now;
            _context.Categories.Update(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(CategoryIndex));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var data = await _context.Categories.FindAsync(id);
            if (data == null) return NotFound();

            data.IsDeleted = true;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(CategoryIndex));
        }

        // =========================
        // TOPIC CRUD + IMAGE
        // =========================
        public async Task<IActionResult> TopicIndex(string search, int page = 1)
        {
            var query = _context.Topics
                .Include(t => t.Category)
                .Where(t => !t.IsDeleted);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(t => t.TopicName.Contains(search));

            int totalData = await query.CountAsync();

            var data = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * PAGE_SIZE)
                .Take(PAGE_SIZE)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = Math.Ceiling(totalData / (double)PAGE_SIZE);
            ViewBag.Search = search;

            return View(data);
        }

        public async Task<IActionResult> CreateTopic()
        {
            ViewBag.Categories = await _context.Categories
                .Where(c => !c.IsDeleted)
                .ToListAsync();

            return View(new Topic());
        }

        // 🔥 CREATE TOPIC + IMAGE
        [HttpPost]
        public async Task<IActionResult> CreateTopic(Topic model, IFormFile? image)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _context.Categories
                    .Where(c => !c.IsDeleted)
                    .ToListAsync();
                return View(model);
            }

            if (image != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                var path = Path.Combine("wwwroot/images", fileName);

                using var stream = new FileStream(path, FileMode.Create);
                await image.CopyToAsync(stream);

                model.TopicPicture = "/images/" + fileName;
            }

            model.CreatedAt = DateTime.Now;
            model.ViewCount = 0;
            model.IsDeleted = false;

            _context.Topics.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(TopicIndex));
        }

        public async Task<IActionResult> EditTopic(int id)
        {
            var data = await _context.Topics.FindAsync(id);
            if (data == null) return NotFound();

            ViewBag.Categories = await _context.Categories
                .Where(c => !c.IsDeleted)
                .ToListAsync();

            return View(data);
        }

        // 🔥 EDIT TOPIC + IMAGE (OPTIONAL)
        [HttpPost]
        public async Task<IActionResult> EditTopic(
            Topic model,
            IFormFile? image,
            string ExistingPicture)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _context.Categories
                    .Where(c => !c.IsDeleted)
                    .ToListAsync();
                return View(model);
            }

            var topic = await _context.Topics.FindAsync(model.TopicId);
            if (topic == null) return NotFound();

            topic.TopicName = model.TopicName;
            topic.Description = model.Description;
            topic.VideoUrl = model.VideoUrl;
            topic.CategoryId = model.CategoryId;
            topic.UpdatedAt = DateTime.Now;

            if (image != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                var path = Path.Combine("wwwroot/images", fileName);

                using var stream = new FileStream(path, FileMode.Create);
                await image.CopyToAsync(stream);

                topic.TopicPicture = "/images/" + fileName;
            }
            else
            {
                topic.TopicPicture = ExistingPicture;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(TopicIndex));
        }

        // =========================
        // DELETE → ARCHIVE
        // =========================
        [HttpPost]
        public async Task<IActionResult> DeleteTopic(int id)
        {
            var topic = await _context.Topics
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.TopicId == id);

            if (topic == null) return NotFound();

            _context.ArchiveTopics.Add(new ArchiveTopic
            {
                TopicName = topic.TopicName,
                Description = topic.Description,
                VideoUrl = topic.VideoUrl,
                CategoryId = topic.CategoryId,
                CreatedAt = topic.CreatedAt,
                ViewCount = topic.ViewCount,
                ArchivedAt = DateTime.Now
            });

            _context.Topics.Remove(topic);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(TopicIndex));
        }
    }
}
