using Microsoft.AspNetCore.Mvc;

namespace Neksara.Controllers
{
  public class ELearningController : Controller
  {
    public IActionResult Index()
    {
      return View();
    }
    // Show categories page (Views/ELearning/Category/Index.cshtml)
    public IActionResult Category()
    {
      return View("Category/Index");
    }

    // Show category details (Views/ELearning/Category/Details.cshtml)
    public IActionResult CategoryDetails(int id)
    {
      // TODO: load category by id and pass model to view
      ViewData["CategoryId"] = id;
      return View("Category/Details");
    }
    
    // Show topics page (Views/ELearning/Topic/Index.cshtml)
    public IActionResult Topic()
    {
      return View("Topic/Index");
    }
    
    // Show topic details (Views/ELearning/Topic/Details.cshtml)
    public IActionResult TopicDetails(int id)
    {
      ViewData["TopicId"] = id;
      return View("Topic/Details");
    }
  }
}