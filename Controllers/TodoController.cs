using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoList2.API.Data;
using TodoList2.API.Dto;
using TodoList2.API.Helpers;
using TodoList2.API.Models;

namespace TodoList2.API.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/todos")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly ITodoRepository _repo;
        private readonly IMapper _mapper;
        private readonly ILog _logger;

        private string _log;

        public TodoController(ITodoRepository repo, IMapper mapper, ILog logger)
        {
            _repo = repo;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet("pages/{page?}/{size?}")]
        public async Task<IActionResult> GetTodos(int userId, int page = 1, int size = 10)
        {
            var userTokenId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                _log = "'GetTodos' Error: 'Unauthorized' on user with id: '" + userId + "' from user with id: '" + userTokenId + "'";
                _logger.Warning(_log);
                return Unauthorized();
            }

            var todos = await _repo.GetTodos(userId);


            var enumerable = todos.ToList();
            var entries = enumerable.Skip((page - 1) * size).Take(size).ToList();
            //var count = enumerable.Count();
            //var totalPages = (int)Math.Ceiling(count / (float)size);
            //var firstPage = 1; // obviously
            //var lastPage = totalPages;
            //var prevPage = page > firstPage ? page - 1 : firstPage;
            //var nextPage = page < lastPage ? page + 1 : lastPage;


            var todoToReturn = _mapper.Map<IEnumerable<TodoForReturnDto>>(entries);

            _log = "'GetTodos' Successful: on user with id: '" + userId + "'";
            _logger.Information(_log);

            return Ok(todoToReturn);
        }

        [HttpGet("{id}", Name = "GetTodo")]
        public async Task<IActionResult> GetTodo(int userId, int id)
        {
            var userTokenId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (userId != userTokenId)
            {
                _log = "'GetTodo' Error: 'Unauthorized' on user with id: '" + userId + "' and on todo with id: '" + id + "' from user with id: '" + userTokenId + "'";
                _logger.Warning(_log);
                return Unauthorized();
            }

            var todoFromRepo = await _repo.GetTodo(id);

            var todo = _mapper.Map<TodoForReturnDto>(todoFromRepo);
            _log = "'GetTodo' Successful: on user with id: '" + userId + "' on todo with id: '" + id + "'";
            _logger.Information(_log);
            return Ok(todo);
        }

        [HttpPost]
        public async Task<IActionResult> AddTodoForUser(int userId, TodoForCreationDto todoForCreationDto)
        {
            var userTokenId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (userId != userTokenId)
            {
                _log = "'AddTodoForUser' Error: 'Unauthorized' on user with id: '" + userId + "' and on todo with 'TaskName': '" + todoForCreationDto.TaskName + "' from user with id: '" + userTokenId + "'";
                _logger.Warning(_log);
                return Unauthorized();
            }

            var userFromRepo = await _repo.GetUser(userId);
            var todo = _mapper.Map<Todo>(todoForCreationDto);
            todo.UserId = userId;
            todo.User = userFromRepo;
            todo.IsComplete = false;
            todo.IsImportant = false;
            todo.IsInTodayView = false;
            todo.Note = "";
            todo.CreatedAtDateTime = DateTime.Now;
            todo.LastUpdatedAtDateTime = DateTime.Now;
            todo.RemindMeDateTime = DateTime.MaxValue;
            todo.TaskLastDateTime = DateTime.MaxValue;
            userFromRepo.Todos.Add(todo);

            if (await _repo.SaveAll())
            {
                var todoToReturn = _mapper.Map<TodoForReturnDto>(todo);

                _log = "'AddTodoForUser' Successful: on user with id: '" + userId + "' and on todo with id: '" + todoForCreationDto.TaskName + "' from user with id: '" + userTokenId + "'";
                _logger.Information(_log);

                return CreatedAtRoute("GetTodo", new { id = todo.Id }, todoToReturn);
            }

            _log = "'AddTodoForUser' Error: 'Bad Request': on user with id: '" + userId + "' and on todo with id: '" + todoForCreationDto.TaskName + "' from user with id: '" + userTokenId + "'";
            _logger.Warning(_log);
            return BadRequest("Could not add the todo item.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodo(int userId, int id, TodoForUpdateDto todoForUpdateDto)
        {
            var userTokenId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (userId != userTokenId)
            {
                _log = "'UpdateTodo' Error: 'Unauthorized' on user with id: '" + userId + "' and on todo with id: '" + id + "' from user with id: '" + userTokenId + "'";
                _logger.Warning(_log);

                return Unauthorized();
            }

            var todoFromRepo = await _repo.GetTodo(id);
            _mapper.Map(todoForUpdateDto, todoFromRepo);

            if (await _repo.SaveAll())
            {
                _log = "'UpdateTodo' Successful: on user with id: '" + userId + "' and on todo with id: '" + id + "' from user with id: '" + userTokenId + "'";
                _logger.Information(_log);

                return NoContent();
            }

            _log = "'UpdateTodo' Error: on user with id: '" + userId + "' and on todo with id: '" + id + "'  failed on save from user with id: '" + userTokenId + "'";
            _logger.Error(_log);
            throw new Exception($"Updating todo {id} failed on save");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(int userId, int id)
        {
            var userTokenId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (userId != userTokenId)
            {
                _log = "'DeleteTodo' Error: 'Unauthorized' on user with id: '" + userId + "' and on todo with id: '" + id + "' from user with id: '" + userTokenId + "'";
                _logger.Warning(_log);

                return Unauthorized();
            }

            var user = await _repo.GetUser(userId);

            if (user.Todos.All(p => p.Id != id))
                return Unauthorized();

            var todoFromRepo = await _repo.GetTodo(id);
            _repo.Delete(todoFromRepo);

            if (await _repo.SaveAll())
            {
                _log = "'DeleteTodo' Successful: on user with id: '" + userId + "' and on todo with id: '" + id + "' from user with id: '" + userTokenId + "'";
                _logger.Information(_log);

                return Ok();
            }

            _log = "'DeleteTodo' Error: 'Bad Request' on user with id: '" + userId + "' and on todo with id: '" + id + "' from user with id: '" + userTokenId + "'";
            _logger.Warning(_log);
            return BadRequest("Failed to delete the todo");
        }

        [HttpGet("lists/{listName}/pages/{page?}/{size?}", Name = "GetLists")]
        public async Task<IActionResult> GetTodoLists(int userId, string listName, int page, int size)
        {
            var userTokenId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (userId != userTokenId)
            {
                _log = "'GetTodoLists' Error: 'Unauthorized' on user with id: '" + userId + "' and on list with name: '" + listName + "' from user with id: '" + userTokenId + "'";
                _logger.Warning(_log);

                return Unauthorized();
            }

            var todos = await _repo.GetTodos(userId, listName);

            var enumerable = todos.ToList();
            var entries = enumerable.Skip((page - 1) * size).Take(size).ToList();

            var todoToReturn = _mapper.Map<IEnumerable<TodoForReturnDto>>(entries);

            _log = "'GetTodoLists' Successful: on user with id: '" + userId + "' and on list with name: '" + listName + "' from user with id: '" + userTokenId + "'";
            _logger.Information(_log);

            return Ok(todoToReturn);
        }

        [HttpGet("lists/{listName}/count")]
        public async Task<IActionResult> GetTodoListCount(int userId, string listName)
        {
            var userTokenId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (userId != userTokenId)
            {
                _log = "'GetTodoListCount' Error: 'Unauthorized' on user with id: '" + userId + "' and on list with name: '" + listName + "' from user with id: '" + userTokenId + "'";
                _logger.Warning(_log);

                return Unauthorized();
            }

            var count = await _repo.GetListCount(userId, listName);

            _log = "'GetTodoListCount' Successful: on user with id: '" + userId + "' and on list with name: '" + listName + "' from user with id: '" + userTokenId + "'";
            _logger.Information(_log);

            return Ok(count);
        }

    }
}