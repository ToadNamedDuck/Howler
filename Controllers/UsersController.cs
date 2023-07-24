using Howler.Models;
using Howler.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace Howler.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetById(int id)
        {
            User user = _userRepository.GetById(id);
            if(user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet("{email}")]
        [Authorize]
        public IActionResult GetByEmail(string email)
        {
            User user = _userRepository.GetByEmail(email);
            if(user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
    }
}
