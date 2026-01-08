using Microsoft.AspNetCore.Mvc;
using Neksara.Data;
using Neksara.Models;
using System;

public class FeedbackController : Controller
{
    private readonly ApplicationDbContext _context;

    public FeedbackController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult Create(Feedback feedback)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction("Detail", "Learning", new { id = feedback.TargetId });
        }

        // 🛡️ FIX: user boleh anon
        feedback.UserId = null;

        feedback.CreatedAt = DateTime.Now;
        feedback.IsApproved = false;
        feedback.IsVisible = false;

        _context.Feedbacks.Add(feedback);
        _context.SaveChanges();

        return RedirectToAction("Detail", "Learning", new { id = feedback.TargetId });
    }
}
