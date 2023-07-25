using Howler.Models;
using Howler.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Howler.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class PacksController : Controller
    {
        private readonly IPackRepository _packRepository;
        public PacksController(IPackRepository packRepository)
        {
            _packRepository = packRepository;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Pack> packs = _packRepository.GetAllPacks();
            return Ok(packs);
        }
    }
}
