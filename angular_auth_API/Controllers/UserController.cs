using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using angular_auth_API.context;
using angular_auth_API.helpers;
using angular_auth_API.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
            var user = await _authDbContext.Users.FirstOrDefaultAsync(x => x.Username == userObject.Username);

            if (user == null)
            {
                return NotFound(new { Message = "User not found!" });
            }

            if (!passwordHasher.VerifyPassword(userObject.Password, user.Password))
            {
                return BadRequest(new { Message = "Password is Incorrect" });

            }

            user.Token = createJWT(user);

            return Ok(new {
                Token = user.Token,
                Message = "Login Success!" }); 

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
            var pass = checkPasswordStrength(userObject.Password);
            if (!string.IsNullOrEmpty(pass))
            {
                return BadRequest(new { Message = pass.ToString() });
            }


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

        private string checkPasswordStrength(string password)
        {
            StringBuilder sb = new StringBuilder();

            if (password.Length < 8)
            {
                sb.Append("Minumum password length should be 8" + Environment.NewLine);
            }
            if (!(Regex.IsMatch(password,"[a-z]") && Regex.IsMatch(password,"[A-Z]") && Regex.IsMatch(password,"[0-9]")))
            {
                sb.Append("Password should be alphanumeric" + Environment.NewLine);
            }
            if (!(Regex.IsMatch(password, "[!, @, #, $, %, ^, &, *, (, ), \\-, _, +, =, \\[, \\], {, }, \\|, \\, ;, :, ', \", ,, ., <, >, /, ?, `\n]")))
            {
                sb.Append("Password should contain special characters" + Environment.NewLine);
            }
            return sb.ToString();
        }
        //JWT token
        private string createJWT(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            //create key

            var key = Encoding.ASCII.GetBytes("veryverysecret.....");

            //create identity
            var identity = new ClaimsIdentity(new Claim[]
            {
                //payload existing of full names and role
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            });

            //create credentials
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            //create token descriptor

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };


            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            //return as a string
            return jwtTokenHandler.WriteToken(token);
        }

        [HttpGet]

        public async Task<ActionResult<User>> getAllUsers()
        {
            return Ok(await _authDbContext.Users.ToListAsync());
        }


    }
}

