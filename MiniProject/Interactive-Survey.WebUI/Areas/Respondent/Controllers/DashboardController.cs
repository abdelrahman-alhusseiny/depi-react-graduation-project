using Interactive_Survey.Utilities.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Interactive_Survey.WebUI.Areas.Respondent.Controllers
{
    [Authorize(Roles = SD.Role_Respondent)]
    [Area("Respondent")]
    [Route("Respondent/Dashboard")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
