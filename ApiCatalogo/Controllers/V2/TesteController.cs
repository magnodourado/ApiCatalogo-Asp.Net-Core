using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCatalogo.Controllers.V2
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")] //apiVersion já faz parte do contexto Asp.NET core setando como versão
    [ApiController]
    public class TesteController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Content("<html><body><h2>TesteV2Controller - V 2.0 </h2></body></html>", "text/html");
        }
    }
}
