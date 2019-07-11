using System;
using System.ComponentModel.DataAnnotations;

namespace TodoList2.API.Dto
{
    public class UserForRegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 4, ErrorMessage = "You must specify the password between 4 and 8 characters.")]
        public string Password { get; set; }

        public DateTime CreatedAtDateTime { get; set; }
        public DateTime LastActiveAtDateTime { get; set; }

        public UserForRegisterDto()
        {
            CreatedAtDateTime = DateTime.Now;
            LastActiveAtDateTime = DateTime.Now;
        }
    }
}
