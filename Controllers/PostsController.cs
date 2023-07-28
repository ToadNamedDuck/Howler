using Howler.Models;
using Howler.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
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

            if (post == null)
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

                if (senderPack.PrimaryBoardId == board.Id)
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

            if (post == null)
            {
                return NotFound();
            }

            Board board = _boardRepository.GetById(post.BoardId);
            if (board.IsPackBoard)
            {
                if (sender.PackId == null)
                {
                    return Forbid();
                }

                Pack senderPack = _packRepository.GetById((int)sender.PackId);
                if (senderPack.PrimaryBoardId == board.Id)
                {
                    return Ok(post);
                }

                return Forbid();
            }

            return Ok(post);
        }

        [HttpGet]
        public IActionResult Search(string q, bool latestFirst)
        {
            List<Post> posts = _postRepository.Search(q, latestFirst);
            return Ok(posts);
        }

        [HttpPost]
        public IActionResult Post(Post post)
        {
            User sender = GetCurrentUser();
            if (post.UserId != sender.Id || String.IsNullOrWhiteSpace(post.Title))
            {
                return BadRequest();
            }
            Board postBoard = _boardRepository.GetById(post.BoardId);
            if (postBoard == null)
            {
                return BadRequest();
            }
            if (postBoard.IsPackBoard)
            {
                if (sender.PackId == null)
                {
                    return Forbid();
                }
                Pack senderPack = _packRepository.GetById((int)sender.PackId);
                if (senderPack.PrimaryBoardId != postBoard.Id)
                {
                    return Forbid();
                }
                post.CreatedOn = DateTime.Now;
                _postRepository.Add(post);
                return CreatedAtAction(nameof(GetById), new { id = post.Id }, post);
            }
            post.CreatedOn = DateTime.Now;
            _postRepository.Add(post);
            return CreatedAtAction(nameof(GetById), new { id = post.Id }, post);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Post post)
        {
            User sender = GetCurrentUser();
            Post postToUpdate = _postRepository.GetById(post.Id);
            if (String.IsNullOrWhiteSpace(post.Title))
            {
                return BadRequest();
            }
            if (postToUpdate == null)
            {
                return NotFound();
            }
            if (id != post.Id || post.UserId != postToUpdate.UserId || post.UserId != sender.Id || postToUpdate.UserId != sender.Id || post.BoardId != postToUpdate.BoardId || post.CreatedOn != postToUpdate.CreatedOn)
            {
                return BadRequest();
            }

            Board postBoard = _boardRepository.GetById(post.BoardId);
            if (postBoard.IsPackBoard == true)
            {
                if (sender.PackId == null)
                {
                    return Forbid();
                }
                Pack senderPack = _packRepository.GetById((int)sender.PackId);
                if (senderPack.PrimaryBoardId == postBoard.Id)
                {
                    _postRepository.Update(post);
                    return NoContent();
                }
                return Forbid();
            }
            _postRepository.Update(post);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            Post post = _postRepository.GetById(id);
            if(post == null)
            {
                return NotFound();
            }

            User sender = GetCurrentUser();
            Board board = _boardRepository.GetById(post.BoardId);

            if (post.UserId == sender.Id || board.BoardOwnerId == sender.Id)
            {
                _postRepository.Delete(post.Id);
                return NoContent();
            }
            return Forbid();
        }

        [HttpGet]
        public IActionResult GetAllPosts(bool latestFirst)
        {
            return Ok(_postRepository.GetAllPosts(latestFirst));
        }

        private User GetCurrentUser()
        {
            var firebaseUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return _userRepository.GetByFirebaseId(firebaseUserId);
        }
    }
}
