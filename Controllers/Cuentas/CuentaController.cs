using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PerrosApp.Controllers.Cuentas
{
    public class CuentaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}