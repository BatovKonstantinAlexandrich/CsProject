using Microsoft.AspNetCore.Mvc;

namespace WebApi_CommonModel.Controllers
{
    
    public class MvcController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult NotFound()
        {
            return View();
        }

        public IActionResult Error(string model)
        {
            object data = String.IsNullOrWhiteSpace(model)?null:model.FromJson<object>();
            return View(data);
        }
    }
}
