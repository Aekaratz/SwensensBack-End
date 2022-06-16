using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SwensenAPI.Models;
using SwensenAPI.Models.Request;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SwensenAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private swensenContext swensenContext = new swensenContext();

        private readonly IConfiguration _configuration;
        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest register)
        {
            try
            {
                var chkUser = await swensenContext.User.FirstOrDefaultAsync(a=>a.Email.Equals(register.PersonData.Email));
                if (chkUser != null)
                {
                    return Ok(new
                    {
                        status = 2,
                        msg = "duplicate user"
                    });
                }
                await swensenContext.User.AddAsync(new User
                {
                    Birthday = register.PersonData.Birthday.Date,
                    Sex = register.PersonData.Gender,
                    Email = register.PersonData.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(register.PersonData.Password),
                    Name = register.PersonData.Name,
                    Lname = register.PersonData.Lastname,
                    Phone = register.PersonData.Phone,
                });
                await swensenContext.SaveChangesAsync();

                await swensenContext.UserAddress.AddAsync(new UserAddress
                {
                    Email = register.PersonData.Email,
                    AddressName = register.PersonAddress.Address,
                    Tambon = register.PersonAddress.TambonID.ToString(),
                    District = register.PersonAddress.DistrictID.ToString(),
                    Province = register.PersonAddress.CountyID.ToString(),
                    Zipcode = register.PersonAddress.Postalcode.ToString(),
                });
                await swensenContext.SaveChangesAsync();

                return Ok(new
                {
                    status = 1,
                    msg = "OK"
                });
            }
            catch
            {
                return BadRequest(new
                {
                    status = 0,
                    msg = "BadRequest"
                });
            }
        }

        [HttpGet("Authen/{email}/{password}")]
        public async Task<IActionResult> Authen(string email, string password)
        {
            try
            {
                var chkEmail = await swensenContext.User.FirstOrDefaultAsync(a => a.Email.Equals(email));
                var dd = BCrypt.Net.BCrypt.Verify(password, chkEmail.Password);
                if (chkEmail == null || !BCrypt.Net.BCrypt.Verify(password, chkEmail.Password))
                {
                    return Ok(new
                    {
                        status = 0,
                        msg = "Username or password is invalid.",
                        data = new { }
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = 1,
                        msg = "OK",
                        data = new
                        {
                            token = GetToken(email),
                            fullNmae = chkEmail.Name + ' ' + chkEmail.Lname,
                            email = email
                        }
                    });
                }

            }
            catch
            {
                return BadRequest();
            }
        }


        private string GetToken(string email)
        {

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(ClaimTypes.Role,"1"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            // var expires = DateTime.Now.AddHours(Convert.ToDouble(_configuration["JwtExpireSeconds"]));
            var expires = DateTime.Now.AddHours(Convert.ToInt32(_configuration["JwtExpireSeconds"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["JwtIssuer"],
                audience: _configuration["JwtAudience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
