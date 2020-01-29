using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Interview_Suraj.Controllers
{
    [Route("api/[controller]")]
    public class CallbackController : Controller
    {     
        // POST api/values
        [HttpPost]
        public string Post([FromQuery]string code)
        {
            return code;
        }
    }
}
