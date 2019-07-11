using System;
using System.Collections.Generic;

namespace TodoList2.API.Dto
{
    public class UserForReturnDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public DateTime CreatedAtDateTime { get; set; }
        public DateTime LastActiveAtDateTime { get; set; }
        public ICollection<TodoForReturnDto> Todos { get; set; }
    }
}
