﻿using Howler.Models;
using Howler.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Howler.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    //Probably add a Does User Exist that returns some sort of true or false value.
    //Most of these shouldn't return an email. lol Would be nice to have pack tied into the user obj. Really considering removing email from user obj.
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IPackRepository _packRepository;

        public UsersController(IUserRepository userRepository, IPackRepository packRepository)
        {
            _userRepository = userRepository;
            _packRepository = packRepository;
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            BarrenUser user = _userRepository.GetById(id);
            if(user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet("{email}")]
        public IActionResult GetByEmail(string email)
        {
            BarrenUser user = _userRepository.GetByEmail(email);
            if(user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet("{firebaseId}")]
        public IActionResult GetByFirebaseId (string firebaseId)
        {
            User user = _userRepository.GetByFirebaseId(firebaseId);
            if(user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet("{id}")]
        public IActionResult GetByIdWithPosts (int id)
        {
            UserWithPosts user = _userRepository.GetByIdWithPosts(id);
            if(user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet]
        public IActionResult GetByPackId(int packId)
        {
            List<BarrenUser> users = _userRepository.GetByPackId(packId);
            return Ok(users);
        }

        [HttpPost]
        public IActionResult Post(User user)
        {
            BarrenUser userWithSameEmail = _userRepository.GetByEmail(user.Email);
            if (userWithSameEmail != null)
            {
                ObjectResult response = new ObjectResult(new { title = "Already Exists", status = 420, message = $"A user with email '{user.Email}' already exists in the database" });
                response.StatusCode = 420;
                return response;
            }
            user.DateCreated = DateTime.Now;
            _userRepository.Add(user);
            return CreatedAtAction(
                nameof(GetByFirebaseId),
                new { firebaseId = user.FirebaseId },
                user);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, BarrenUser user)
        {
            User currentUser = GetCurrentUser();
            if(id != user.Id)
            {
                return BadRequest();
            }
            if(user.PackId != null)
            {
                Pack userUpdatedPack = _packRepository.GetById((int)user.PackId);
                if(userUpdatedPack == null)
                {
                    return BadRequest();
                }
            }
            if(id != currentUser.Id)
            {
                return Unauthorized();
            }
            _userRepository.Update(user);
            return NoContent();

        }


        //This will grab the user's FirebaseId from the global user object, which is contained in the JWT of the request. lol. Can use to verify a user is editing their own profile.
        private User GetCurrentUser()
        {
            var firebaseUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return _userRepository.GetByFirebaseId(firebaseUserId);
        }
    }
}
