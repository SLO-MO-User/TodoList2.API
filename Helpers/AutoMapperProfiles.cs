using AutoMapper;
using TodoList2.API.Dto;
using TodoList2.API.Models;

namespace TodoList2.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<UserForRegisterDto, User>();

            CreateMap<UserForUpdateDto, User>();

            CreateMap<UserForReturnDto, User>();

            CreateMap<User, UserForReturnDto>();

            CreateMap<Todo, TodoForReturnDto>();

            CreateMap<TodoForUpdateDto, Todo>();

            CreateMap<TodoForCreationDto, Todo>();
        }
    }
}