using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SmartAdmin.Controllers
{
    public class SetupController : Controller
    {
        public IActionResult Setup()
        {
            return View();
        }
    }
}