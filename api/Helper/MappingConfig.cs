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
            CreateMap<Room, CreateRoomDTO>().ReverseMap();
            CreateMap<Room, RoomDTO>().ReverseMap();
            CreateMap<Models.TaskModel, TaskDTO>().ReverseMap();
            CreateMap<TaskModel, TaskDTO>()
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));
            CreateMap<Models.TaskModel, CreateTaskDTO>().ReverseMap();

        }
    }
}