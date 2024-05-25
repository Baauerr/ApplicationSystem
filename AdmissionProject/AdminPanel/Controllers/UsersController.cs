using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Users()
        {
            return View();
        }
    }
}
