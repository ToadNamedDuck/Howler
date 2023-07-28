using Howler.Models;
using Howler.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
        private readonly IBoardRepository _boardRepository;
        public PacksController(IPackRepository packRepository, IUserRepository userRepository, IBoardRepository boardRepository)
        {
            _packRepository = packRepository;
            _userRepository = userRepository;
            _boardRepository = boardRepository;
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
            if(pack.PackLeaderId != currentUser.Id || string.IsNullOrWhiteSpace(pack.Name))
            {
                return BadRequest();
            }
            _boardRepository.GeneratePackBoard(pack);
            _packRepository.Add(pack);
            return CreatedAtAction(nameof(GetById), new {id = pack.Id}, pack);
            
        }

        //I never made update yesterday (Today is 7/26/23) ! Oops!
        [HttpPut("{id}")]
        public IActionResult Update(int id, Pack pack)
        {
            User sender = GetCurrentUser();
            BarrenUser updatedPackLeader = _userRepository.GetById(pack.PackLeaderId); 
            Pack packBeingUpdated = _packRepository.GetById(id);
            Pack packWithSameName = _packRepository.ExactSearch(pack.Name);
            if (string.IsNullOrWhiteSpace(pack.Name))
            {
                return BadRequest();
            }
            if(packBeingUpdated != null)
            {

                if(packBeingUpdated.PackLeaderId != sender.Id) 
                {
                    return Forbid();
                }
                if(packBeingUpdated.PrimaryBoardId != pack.PrimaryBoardId)
                {
                    return BadRequest();
                }
                if (sender.PackId != packBeingUpdated.Id || sender.PackId != pack.Id || pack.Id != packBeingUpdated.Id || updatedPackLeader == null)
                {
                    return BadRequest();
                }
                if(updatedPackLeader.PackId != pack.Id || updatedPackLeader.PackId != packBeingUpdated.Id)
                {
                    ObjectResult response = new ObjectResult(new { title = "Can't Assign Pack to Non-Member", status = 470, message = $"{updatedPackLeader.DisplayName} is not a member of this pack, and cannot be set as the leader!" });
                    response.StatusCode = 470;
                    return response;
                }
                if(packWithSameName != null && packWithSameName.Id != pack.Id)
                {
                    ObjectResult response = new ObjectResult(new { title = "Already Exists", status = 420, message = $"A pack named '{pack.Name}' already exists in the database" });
                    response.StatusCode = 420;
                    return response;
                }

                _packRepository.Edit(pack);
                return NoContent();
            }
            return NotFound();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            User currentUser = GetCurrentUser();
            Pack packToDelete = _packRepository.GetById(id);
            if(packToDelete != null)
            {
                if(packToDelete.PackLeaderId != currentUser.Id)
                {
                    return Forbid();
                }
                if (packToDelete.PrimaryBoardId != null)
                {
                    _boardRepository.Delete((int)packToDelete.PrimaryBoardId);
                }
                _packRepository.Delete(id);
                return NoContent();
            }
            return NotFound();
        }

        [HttpGet]
        public IActionResult Search(string q) 
        {
            return Ok(_packRepository.Search(q));
        }

        private User GetCurrentUser()
        {
            var firebaseUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return _userRepository.GetByFirebaseId(firebaseUserId);
        }
    }
}
