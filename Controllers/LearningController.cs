using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Neksara.Data;

public class LearningController : Controller
{
    private readonly ApplicationDbContext _context;
    private const int PageSize = 6;

    public LearningController(ApplicationDbContext context)
    {
        _context = context;
    }

    // ================= LIST + FILTER + PAGINATION =================
    public async Task<IActionResult> Index(int? categoryId, int page = 1)
    {
        var categories = await _context.Categories
            .Where(c => !c.IsDeleted)
            .ToListAsync();

        var query = _context.Topics
            .Include(t => t.Category)
            .Where(t => !t.IsDeleted);

        if (categoryId != null)
            query = query.Where(t => t.CategoryId == categoryId);

        var totalItems = await query.CountAsync();

        var topics = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        ViewBag.Category = categories;
        ViewBag.CurrentCategory = categoryId;
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

        return View(topics);
    }

    // ================= DETAIL TOPIK + RATING =================
    public async Task<IActionResult> Detail(int id)
    {
        var topic = await _context.Topics
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.TopicId == id && !t.IsDeleted);

        if (topic == null)
            return NotFound();

        topic.ViewCount++;
        await _context.SaveChangesAsync();

        // ⭐ AVG RATING
        var avgRating = await _context.Feedbacks
            .Where(f => f.TargetType == "Topic"
                && f.TargetId == id
                && f.IsApproved
                && f.IsVisible)
            .AverageAsync(f => (double?)f.Rating) ?? 0;

        ViewBag.AvgRating = Math.Round(avgRating, 1);

        return View(topic);
    }
}
