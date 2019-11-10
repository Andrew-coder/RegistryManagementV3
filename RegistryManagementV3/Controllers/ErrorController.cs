using Microsoft.AspNetCore.Mvc;

namespace RegistryManagementV3.Controllers
{
    [Route("/Error")]
    public class ErrorController : Controller
    {

        public IActionResult AppError()
        {
            return View("Error");
        }
    }
}