using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using angular_auth_API.context;
using angular_auth_API.helpers;
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
            //check username
            if (await checkUsernameExistAsync(userObject.Username))
            {
                return BadRequest(new { Message = "Username already exists!" });
            }
            //check email
            if (await checkEmailExistAsync(userObject.Email))
            {
                return BadRequest(new { Message = "Email already exists!" });
            }


            //check passcode strength

            //hash passcode
            userObject.Password = passwordHasher.hashPassword(userObject.Password);

            userObject.Role = "User";//as default
            userObject.Token = "";//as default

            //add to the db and save
            await _authDbContext.Users.AddAsync(userObject);
            await _authDbContext.SaveChangesAsync();


            return Ok(new { Message = "User Registered!" });
        }

        // method for checking username
        private async Task<bool> checkUsernameExistAsync(string username)
        {
            return await _authDbContext.Users.AnyAsync(x => x.Username == username);
        }
        // method for checking email
        private async Task<bool> checkEmailExistAsync(string email)
        {
            return await _authDbContext.Users.AnyAsync(x => x.Email == email);
        }


    }
}

