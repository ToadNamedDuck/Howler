using Azure;
using Howler.Models;
using Howler.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;

namespace Howler.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class BoardsController : Controller
    {
        private readonly IBoardRepository _boardRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPackRepository _packRepository;

        public BoardsController(IBoardRepository boardRepository, IUserRepository userRepository, IPackRepository packRepository)
        {
            _boardRepository = boardRepository;
            _userRepository = userRepository;
            _packRepository = packRepository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            List<Board> boards = _boardRepository.GetAllBoards();
            return Ok(boards);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            Board board = _boardRepository.GetById(id);
            if(board == null)
            {
                return NotFound();
            }
            if(board.IsPackBoard == true)
            {
                User sender = GetCurrentUser();
                if(sender.PackId == null)
                {
                    return Forbid();
                }
                else
                {
                    Pack senderPack = _packRepository.GetById((int)sender.PackId);//get the current users pack
                    if(senderPack.PrimaryBoardId == board.Id)//check to see if the pack's primary board id matches the one we found earlier
                    {
                        return Ok(board);//If the board is and the pack.PrimaryBoardId match, we're good to view it
                    }
                    return Forbid();
                }
            }
            return Ok(board);
        }
        [HttpPost]
        public IActionResult Post(Board board)
        {
            Board boardWithSameName = _boardRepository.ExactSearch(board.Name);
            User sender = GetCurrentUser();
            if (boardWithSameName != null)
            {
                ObjectResult response = new ObjectResult(new { title = "Already Exists", status = 420, message = $"A board named '{boardWithSameName.Name}' already exists in the database" });
                response.StatusCode = 420;
                return response;
            }
            if(sender.Id != board.BoardOwnerId)
            {
                return BadRequest();
            }
            _boardRepository.Add(board);
            return CreatedAtAction(nameof(GetById), new { id = board.Id }, board);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Board board)
        {
            User sender = GetCurrentUser();
            Board boardToEdit = _boardRepository.GetById(id);
            if(boardToEdit == null)
            {
                return NotFound();
            }
            if(board.IsPackBoard != boardToEdit.IsPackBoard)
            {
                return BadRequest();
            }
            if(boardToEdit.BoardOwnerId != sender.Id)
            {
                return Forbid();
            }
            if(board.Id != boardToEdit.Id || board.Id != id)
            {
                return BadRequest();
            }
            _boardRepository.Update(board);
            return NoContent();
        }

        private User GetCurrentUser()
        {
            var firebaseUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return _userRepository.GetByFirebaseId(firebaseUserId);
        }
    }
}
