using Howler.Models;
using Howler.Repositories;
using Microsoft.AspNetCore.Authorization;
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

        private User GetCurrentUser()
        {
            var firebaseUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return _userRepository.GetByFirebaseId(firebaseUserId);
        }
    }
}
