using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgravitaeWebExtension.Controllers.Ewallet
{
    public class PaymentMethodController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
