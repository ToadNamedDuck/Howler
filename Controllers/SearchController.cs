using Howler.Models;
using Howler.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Howler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SearchController : ControllerBase
    {
        private readonly IPackRepository _packRepository;
        private readonly IBoardRepository _boardRepository;
        private readonly IPostRepository _postRepository;
        private readonly ICommentRepository _commentRepository;

        public SearchController(IPackRepository packRepository, IBoardRepository boardRepository, IPostRepository postRepository, ICommentRepository commentRepository)
        {
            _packRepository = packRepository;
            _boardRepository = boardRepository;
            _postRepository = postRepository;
            _commentRepository = commentRepository;
        }
        [HttpGet]
        public IActionResult Search(string q, bool latestFirst)
        {
            //need to make a model that has a List<> for packs, boards, posts, and comments in it so we can search everything all at once :D
            OmniSearchResult results = new()
            {
                Packs = _packRepository.Search(q),
                Boards = _boardRepository.Search(q),
                Posts = _postRepository.Search(q, latestFirst),
                Comments = _commentRepository.Search(q, latestFirst)
            };


            return Ok(results);
        }
    }
}
