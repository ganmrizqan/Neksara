using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Neksara.Data;
using Neksara.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Neksara.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminFeedbackController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminFeedbackController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // LIST PENDING FEEDBACK
        // =========================
        public async Task<IActionResult> Index(int? categoryId, int page = 1)
            {
                int pageSize = 10;

            var categories = await _context.Categories.ToListAsync();   
                ViewBag.Categories = categories;
                ViewBag.CategoryId = categoryId;

                var query =
                    from f in _context.Feedbacks
                    join t in _context.Topics on f.TargetId equals t.TopicId
                    join c in _context.Categories on t.CategoryId equals c.CategoryId
                    where f.TargetType == "Topic" && !t.IsDeleted
                    select new AdminFeedbackVM
                    {
                        FeedbackId = f.FeedbackId,
                        Name = f.Name,
                        Role = f.Role,
                        TopicName = t.TopicName,
                        CategoryName = c.CategoryName,
                        Rating = f.Rating,
                        Description = f.Description
                    };

                if (categoryId != null)
                    query = query.Where(x => x.CategoryName != null && 
                                            _context.Categories
                                            .Any(c => c.CategoryId == categoryId && c.CategoryName == x.CategoryName));

                int totalItems = await query.CountAsync();

                var data = await query
                    .OrderByDescending(x => x.FeedbackId)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                ViewBag.Categories = categories;
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                ViewBag.CategoryId = categoryId;

                return View(data);
        }


        // =========================
        // USER - CREATE FEEDBACK
        // =========================
        [HttpPost]
        public async Task<IActionResult> Create(Feedback feedback)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            feedback.CreatedAt = DateTime.Now;
            feedback.IsApproved = false;
            feedback.IsVisible = false;

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        // =========================
        // ADMIN - APPROVE
        // =========================
        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null) return NotFound();

            feedback.IsApproved = true;
            feedback.IsVisible = true;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // ADMIN - HIDE / REJECT
        // =========================
        [HttpPost]
        public async Task<IActionResult> Hide(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null) return NotFound();

            feedback.IsVisible = false;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
