using Howler.Models;
using Howler.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

namespace Howler.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IPostRepository _postRepository;
        private readonly IBoardRepository _boardRepository;
        private readonly IPackRepository _packRepository;
        private readonly IUserRepository _userRepository;

        public CommentsController(ICommentRepository commentRepository, IPostRepository postRepository, IBoardRepository boardRepository, IPackRepository packRepository, IUserRepository userRepository)
        {
            _commentRepository = commentRepository;
            _postRepository = postRepository;
            _boardRepository = boardRepository;
            _packRepository = packRepository;
            _userRepository = userRepository;
        }


        [HttpGet]
        public IActionResult Search(string q, bool latestFirst)
        {
            return Ok(_commentRepository.Search(q, latestFirst));
        }

        [HttpPost]
        public IActionResult Post(Comment comment)
        {
            comment.CreatedOn = DateTime.Now;

            if (String.IsNullOrWhiteSpace(comment.Content))
            {
                return BadRequest();
            }
            User sender = GetCurrentUser();

            Post commentedOn = _postRepository.GetById(comment.PostId);
            if (commentedOn == null || comment.UserId != sender.Id)
            {
                return BadRequest();
            }

            Board postBoard = _boardRepository.GetById(commentedOn.BoardId);
            if (postBoard.IsPackBoard)
            {
                if(sender.PackId == null)
                {
                    return Unauthorized();
                }
                Pack senderPack = _packRepository.GetById((int)sender.PackId);

                if (senderPack.PrimaryBoardId == postBoard.Id)
                {
                    _commentRepository.Add(comment);
                    return StatusCode(201, comment);
                }
                return Unauthorized();
            }
            _commentRepository.Add(comment);
            return StatusCode(201, comment);
        }
        private User GetCurrentUser()
        {
            var firebaseUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return _userRepository.GetByFirebaseId(firebaseUserId);
        }
    }
}
