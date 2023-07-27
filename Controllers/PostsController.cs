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

        [HttpGet]
        public IActionResult Search(string q)
        {
            List<Post> posts = _postRepository.Search(q);
            return Ok(posts);
        }

        [HttpPost]
        public IActionResult Post(Post post)
        {
            User sender = GetCurrentUser();
            if(post.UserId != sender.Id)
            {
                return BadRequest();
            }
            Board postBoard = _boardRepository.GetById(post.BoardId);
            if(postBoard == null)
            {
                return BadRequest();
            }
            if (postBoard.IsPackBoard)
            {
                if(sender.PackId == null)
                {
                    return Forbid();
                }
                Pack senderPack = _packRepository.GetById((int)sender.PackId);
                if(senderPack.PrimaryBoardId != postBoard.Id)
                {
                    return Forbid();
                }
                _postRepository.Add(post);
                return CreatedAtAction(nameof(GetById), new { id = post.Id }, post);
            }
            _postRepository.Add(post);
            return CreatedAtAction(nameof(GetById), new { id = post.Id }, post);
        }
        private User GetCurrentUser()
        {
            var firebaseUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return _userRepository.GetByFirebaseId(firebaseUserId);
        }
    }
}
