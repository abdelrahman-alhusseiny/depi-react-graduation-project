using Interactive_Survey.Utilities.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Interactive_Survey.WebUI.Areas.Creator.Controllers
{
    [Authorize(Roles = SD.Role_Creator)]
    [Area("Creator")]
    [Route("Creator/Dashboard")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
