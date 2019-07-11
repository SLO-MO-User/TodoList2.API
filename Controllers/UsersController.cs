using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodoList2.API.Data;
using TodoList2.API.Dto;
using TodoList2.API.Helpers;

namespace TodoList2.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ITodoRepository _repo;
        private readonly IMapper _mapper;
        private readonly ILog _logger;

        private string log;

        public UsersController(ITodoRepository repo, IMapper mapper, ILog logger)
        {
            _repo = repo;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var userTokenId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (id != userTokenId)
            {
                log = "User with id: '" + userTokenId + "' tried unauthorized 'GetUser' attempt on user with id: '" + id + "'";
                _logger.Warning(log);
                return Unauthorized();
            }

            var user = await _repo.GetUser(id);
            var userToReturn = _mapper.Map<UserForReturnDto>(user);

            log = "User with id: '" + userTokenId + "' has succeed 'GetUser' attempt on user with id: '" + id + "'";
            _logger.Information(log);

            return Ok(userToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto)
        {
            var userTokenId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (id != userTokenId)
            {
                log = "User with id: '" + userTokenId + "' tried unauthorized 'UpdateUser' attempt on user with id: '" + id + "'";
                _logger.Warning(log);

                return Unauthorized();
            }

            var userFromRepo = await _repo.GetUser(id);
            _mapper.Map(userForUpdateDto, userFromRepo);



            if (await _repo.SaveAll())
            {
                log = "User with id: '" + userTokenId + "' has succeed 'UpdateUser' attempt on user with id: '" + id + "'";
                _logger.Information(log);

                return NoContent();
            }


            log = "User with id: '" + userTokenId + "' has tried 'UpdateUser' attempt on user with id: '" + id + "' failed on save";
            _logger.Error(log);
            throw new Exception($"Updating user {id} failed on save");
        }
    }
}