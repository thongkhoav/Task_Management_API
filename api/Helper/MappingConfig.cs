using api.Dtos;
using api.Models;
using AutoMapper;

namespace api.Helper
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {

            CreateMap<ApplicationUser, UserDTO>().ReverseMap();
            CreateMap<Room, RoomDTO>().ReverseMap();
            CreateMap<Models.Task, TaskDTO>().ReverseMap();
        }
    }
}