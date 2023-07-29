using Howler.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    }
}
