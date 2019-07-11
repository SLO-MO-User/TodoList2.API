using System;
using System.Collections.Generic;

namespace TodoList2.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateTime CreatedAtDateTime { get; set; }
        public DateTime LastActiveAtDateTime { get; set; }
        public ICollection<Todo> Todos { get; set; }
    }
}
