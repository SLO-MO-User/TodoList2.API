using System.Collections.Generic;
using System.Threading.Tasks;
using TodoList2.API.Models;

namespace TodoList2.API.Data
{
    public interface ITodoRepository
    {
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveAll();
        Task<User> GetUser(int id);
        Task<Todo> GetTodo(int id);
        Task<IEnumerable<Todo>> GetTodos(int userId);
        Task<IEnumerable<Todo>> GetTodos(int userId, string listName);
        Task<int> GetListCount(int userId, string listName);
    }
}
