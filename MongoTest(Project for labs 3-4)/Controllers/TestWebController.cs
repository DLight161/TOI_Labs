using Microsoft.AspNetCore.Mvc;
using MongoTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MongoTest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestWebController : Controller
    {
        private readonly ProductService db;
        public TestWebController(ProductService context)
        {
            db = context;
        }

        [HttpGet("Get")]
        public IEnumerable<Product> Get()
        {
            var phones = db.GetProducts(null, null, null);
            return phones.Result;
        }

        [HttpGet("Create")]
        public void Create(string name, string company, int price)
        {
            db.Create(new Product { Name = name, Company = company, Price = price  });
        }
    }
}
