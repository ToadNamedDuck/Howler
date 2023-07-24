using Howler.Models;
using Howler.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System;
using System.Security.Claims;

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



        //This will grab the user's FirebaseId from the global user object, which is contained in the JWT of the request. lol. Can use to verify a user is editing their own profile.
        //private User GetCurrentUserProfile()
        //{
        //    var firebaseUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
        //    return _userRepository.GetByFirebaseUserId(firebaseUserId);
        //}
    }
}
