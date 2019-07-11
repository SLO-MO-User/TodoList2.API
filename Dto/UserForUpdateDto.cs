using System;
using System.Collections.Generic;
using TodoList2.API.Models;

namespace TodoList2.API.Dto
{
    public class UserForUpdateDto
    {
        public DateTime LastActiveAtDateTime { get; set; }

        public UserForUpdateDto()
        {
            LastActiveAtDateTime = DateTime.Now;
        }
    }
}
