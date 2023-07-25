using Howler.Models;
using Howler.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Claims;

namespace Howler.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class PacksController : Controller
    {
        private readonly IPackRepository _packRepository;
        private readonly IUserRepository _userRepository;
        public PacksController(IPackRepository packRepository, IUserRepository userRepository)
        {
            _packRepository = packRepository;
            _userRepository = userRepository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            List<Pack> packs = _packRepository.GetAllPacks();
            return Ok(packs);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            Pack pack = _packRepository.GetById(id);
            if(pack == null)
            {
                return NotFound();
            }
            return Ok(pack);
        }

        [HttpPost]
        public IActionResult Post(Pack pack)
        {
            Pack foundPackWithSameName = _packRepository.ExactSearch(pack.Name);
            User currentUser = GetCurrentUser();
            if(foundPackWithSameName != null)
            {
                ObjectResult response = new ObjectResult(new { title = "Already Exists", status = 420, message = $"A pack named '{pack.Name}' already exists in the database" });
                response.StatusCode = 420;
                return response;
            }
            if(currentUser.PackId != null)
            {
                ObjectResult response = new ObjectResult(new { title = "User Already In a Pack", status = 469, message = $"The current user - {currentUser.DisplayName} - is already in a pack, and can't create another without leaving the current pack." });
                response.StatusCode = 469;
                return response;
            }
            if(pack.PackLeaderId != currentUser.Id)
            {
                return BadRequest();
            }
            _packRepository.Add(pack);
            return CreatedAtAction(nameof(GetById), new {id = pack.Id}, pack);
            
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            User currentUser = GetCurrentUser();
            Pack packToDelete = _packRepository.GetById(id);
            if(packToDelete.PackLeaderId != currentUser.Id)
            {
                return Forbid();
            }
            _packRepository.Delete(id);
            return NoContent();
        }

        private User GetCurrentUser()
        {
            var firebaseUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return _userRepository.GetByFirebaseId(firebaseUserId);
        }
    }
}
