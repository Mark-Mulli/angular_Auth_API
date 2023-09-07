using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using angular_auth_API.context;
using angular_auth_API.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace angular_auth_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UserController : Controller
    {
        private readonly appDbContext _authDbContext;

        public UserController(appDbContext appDbContext)
        {
            _authDbContext = appDbContext;
        }

        [HttpPost("authenticate")]

        public async Task<IActionResult> Authenticate([FromBody] User userObject)
        {
            if (userObject == null)
            {
                return BadRequest();
            }

            //username and password matches
            var user = await _authDbContext.Users.FirstOrDefaultAsync(x => x.Username == userObject.Username && x.Password == userObject.Password);

            if (user == null)
            {
                return NotFound(new { Message = "User not found!" });
            }

            return Ok(new { Message = "Login Success!" });

        }

        [HttpPost("register")]

        public async Task<IActionResult> RegisterUser([FromBody] User userObject)
        {
            if (userObject == null)
            {
                return BadRequest();
            }

            await _authDbContext.Users.AddAsync(userObject);
            await _authDbContext.SaveChangesAsync();


            return Ok(new { Message = "User Registered!" });
        }


    }
}

