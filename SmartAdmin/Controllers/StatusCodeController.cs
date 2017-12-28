using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Diagnostics;

namespace SmartAdmin.Controllers
{
    public class StatusCodeController : Controller
    {
        private readonly ILogger _logger;
        public StatusCodeController(ILogger<StatusCodeController> logger)
        {
            this._logger = logger;
        }



        [HttpGet("/StatusCode/{statusCode}")]
        public IActionResult Index(int statusCode)
        {
            var reExecute = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            if ( reExecute != null) { 
            _logger.LogInformation($"Unexpected Status Code: {statusCode}, OriginalPath: {reExecute.OriginalPath}");
            }else
            { _logger.LogInformation(statusCode, "Generic Error", statusCode );
            }
            ViewData["logger"] = _logger;
            return View(statusCode);
        }
    }
}