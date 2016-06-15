using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OAuth.WebApi.Controllers
{

    [Authorize]
    public class CustomerController : ApiController
    {
        private List<Customer> customers;
        public CustomerController()
        {
            this.customers = new List<Customer>
                                 {
                                     new Customer { Id = 1, Name = "Amitabh" },
                                     new Customer { Id = 2, Name = "Shahrukh" },
                                     new Customer { Id = 1, Name = "Aamir" }
                                 };

        }

        [HttpGet]
        public IHttpActionResult Get()
        {
            return this.Ok(this.customers);
        }

        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            if (id == 0)
            {
                return this.BadRequest();
            }
            var customer = this.customers.FirstOrDefault(c => c.Id == id);
            if (customer == null)
            {
                return this.NotFound();
            }
            return this.Ok(customer);
        }

    }

    internal class Customer
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
