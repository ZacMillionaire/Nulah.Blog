using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Nulah.Blog.Areas.Errors.Controllers {
    [Area("Errors")]
    public class ErrorController : Controller {
        [Route("~/Error/{ErrorCode}")]
        public IActionResult CatchAllErrors(int ErrorCode) {
            ViewBag.ErrorCode = ErrorCode;
            return View();
        }
    }
}