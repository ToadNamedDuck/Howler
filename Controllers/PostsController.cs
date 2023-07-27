using Howler.Models;
using Howler.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Howler.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class PostsController : ControllerBase
    {
        private readonly IBoardRepository _boardRepository;
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPackRepository _packRepository;

        public PostsController(IPostRepository postRepository, IBoardRepository boardRepository, IUserRepository userRepository, IPackRepository packRepository)
        {
            _boardRepository = boardRepository;
            _postRepository = postRepository;
            _userRepository = userRepository;
            _packRepository = packRepository;
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            User sender = GetCurrentUser();
            Post post = _postRepository.GetById(id);

            if(post == null)
            {
                return NotFound();
            }
            Board board = _boardRepository.GetById(post.BoardId);

            if (board.IsPackBoard == true)
            {
                if (sender.PackId == null)
                {
                    return Forbid();
                }

                Pack senderPack = _packRepository.GetById((int)sender.PackId);

                if(senderPack.PrimaryBoardId == board.Id)
                {
                    return Ok(post);
                }
                return Forbid();
            }
            return Ok(post);
        }

        [HttpGet("{id}")]
        public IActionResult GetWithComments(int id)
        {
            PostWithComments post = _postRepository.GetWithComments(id);
            User sender = GetCurrentUser();

            if(post == null)
            {
                return NotFound();
            }

            Board board = _boardRepository.GetById(post.BoardId);
            if(board.IsPackBoard)
            {
                if (sender.PackId == null)
                {
                    return Forbid();
                }

                Pack senderPack = _packRepository.GetById((int)sender.PackId);
                if(senderPack.PrimaryBoardId == board.Id)
                {
                    return Ok(post);
                }

                return Forbid();
            }

            return Ok(post);
        }


        private User GetCurrentUser()
        {
            var firebaseUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return _userRepository.GetByFirebaseId(firebaseUserId);
        }
    }
}
