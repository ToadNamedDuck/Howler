﻿using Howler.Models;
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

        [HttpPut("{id}")]
        public IActionResult Update(int id, Comment comment)
        {
            if (String.IsNullOrWhiteSpace(comment.Content))
            {
                return BadRequest();
            }
            User sender = GetCurrentUser();

            Comment originalComment = _commentRepository.GetById(id);
            if(originalComment == null)
            {
                return NotFound();
            }

            Post commentedOn = _postRepository.GetById(comment.PostId);
            if (commentedOn == null || comment.UserId != sender.Id || comment.PostId != commentedOn.Id || comment.Id != id || originalComment.UserId != sender.Id)
            {
                return BadRequest();
            }

            Board postBoard = _boardRepository.GetById(commentedOn.BoardId);
            if (postBoard.IsPackBoard)
            {
                if (sender.PackId == null)
                {
                    return Unauthorized();
                }
                Pack senderPack = _packRepository.GetById((int)sender.PackId);

                if (senderPack.PrimaryBoardId == postBoard.Id)
                {
                    _commentRepository.Update(comment);
                    return NoContent();
                }
                return Unauthorized();
            }
            _commentRepository.Update(comment);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            User sender = GetCurrentUser();
            Comment comment = _commentRepository.GetById(id);
            if(comment == null)
            {
                return NotFound();
            }

            Post post = _postRepository.GetById(comment.PostId);
            Board board = _boardRepository.GetById(post.BoardId);

            if(sender.Id == board.BoardOwnerId || sender.Id == comment.UserId)
            {
                _commentRepository.Delete(id);
                return NoContent();
            }
            return Unauthorized();
        }

        private User GetCurrentUser()
        {
            var firebaseUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return _userRepository.GetByFirebaseId(firebaseUserId);
        }
    }
}
