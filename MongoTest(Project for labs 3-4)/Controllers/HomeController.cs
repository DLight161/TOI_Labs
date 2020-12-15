using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoTest.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MongoTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProductService db;
        public HomeController(ProductService context)
        {
            db = context;
        }
        public async Task<IActionResult> Index(FilterViewModel filter)
        {
            var phones = await db.GetProducts(filter.MinPrice, filter.MaxPrice, filter.Name);
            var model = new IndexViewModel { Products = phones, Filter = filter };
            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product p)
        {
            if (ModelState.IsValid)
            {
                await db.Create(p);
                return RedirectToAction("Index");
            }
            return View(p);
        }
    }
}
