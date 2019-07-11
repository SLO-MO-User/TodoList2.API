using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoList2.API.Models;

namespace TodoList2.API.Data
{
    public class TodoRepository : ITodoRepository
    {
        private readonly DataContext _context;

        public TodoRepository(DataContext context)
        {
            _context = context;
        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.Include(i => i.Todos)
                                           .FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }

        public async Task<Todo> GetTodo(int id)
        {
            var todo = await _context.Todos.FirstOrDefaultAsync(i => i.Id == id);
            return todo;
        }

        public async Task<IEnumerable<Todo>> GetTodos(int userId)
        {
            IEnumerable<Todo> todos = await _context.Todos.Where(t => t.UserId == userId).ToListAsync();
            return todos;
        }

        public async Task<IEnumerable<Todo>> GetTodos(int userId, string listName)
        {
            IEnumerable<Todo> todos = await _context.Todos.Where(t => t.UserId == userId).ToListAsync();

            if (listName == "tasks")
            {
                // return all
            }

            else if (listName == "completed")
            {
                todos = todos.Where(t => t.IsComplete).ToList();
            }

            else if (listName == "planned")
            {
                todos = todos.Where(t => (t.TaskLastDateTime - DateTime.Now).TotalDays <= 7).ToList();
            }

            else if (listName == "important")
            {
                todos = todos.Where(t => t.IsImportant).ToList();
            }

            else if (listName == "today")
            {
                todos = todos.Where(t => t.IsInTodayView).ToList();
            }

            else
            {
                todos = todos.Where(todo => todo.ListName.Equals(listName)).ToList();
            }


            return todos;
        }

        public async Task<int> GetListCount(int userId, string listName)
        {
            var list = await _context.Todos.Where(t => t.UserId == userId).ToListAsync();

            if (listName == "tasks") // tasks
            {
                // return all
            }

            else if (listName == "completed") // completed
            {
                list = list.Where(t => t.IsComplete).ToList();
            }

            else if (listName == "planned") // planned
            {
                list = list.Where(t => (t.TaskLastDateTime - DateTime.Now).TotalDays <= 7).ToList();
            }

            else if (listName == "important") // important
            {
                list = list.Where(t => t.IsImportant).ToList();
            }

            else if (listName == "today") // today
            {
                list = list.Where(t => t.IsInTodayView).ToList();
            }
            else
            {
                list = list.Where(t => t.ListName.Equals(listName)).ToList();
            }

            return list.Count;
        }
    }
}
