using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repo, IConfiguration config)
        {
           _config = config;
            _repo = repo;

        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(USerForRegisterDto uSerForRegisterDto)
        {
            uSerForRegisterDto.Username = uSerForRegisterDto.Username.ToLower();
            if (await _repo.UserExists(uSerForRegisterDto.Username))
                return BadRequest("username already exists");

            var userToCreate = new User
            {
                Username = uSerForRegisterDto.Username
            };
            var createdUser = await _repo.Register(userToCreate, uSerForRegisterDto.Password);
            return StatusCode(201);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLofinDto userForLoginDto)
        {
            var userFromRepo = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);
            if (userFromRepo == null)
                return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username)
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8
            .GetBytes(_config.GetSection("AppSettings:Tokken").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                        Subject = new ClaimsIdentity(claims),
                        Expires = DateTime.Now.AddDays(1),
                        SigningCredentials = creds

            };
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokken = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new {

                tokken = tokenHandler.WriteToken(tokken)
            });
        }


    }
}