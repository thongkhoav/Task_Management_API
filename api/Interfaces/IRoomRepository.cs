using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.Dtos.Room;
using api.Models;

namespace api.Interface
{
    public interface IRoomRepository
    {
        ICollection<Room?> GetRoomsByUserId(Guid userId);
        ICollection<RoomDTO?> GetAllRoom(Guid userId);
        RoomWithOwnerDTO? GetRoomById(Guid roomId);
        bool IsRoomCreator(Guid userId, Guid roomId); // check if user is room
        bool IsRoomMember(Guid roomId, string email);
        bool IsRoomMember(Guid userId, Guid roomId);
        bool IsRoomExist(Guid roomId); // check if user is room
        bool CreateRoom(Guid creatorId, Room room);
        bool UpdateRoom(Room room);
        bool DeleteRoom(Guid roomId);
        bool AddMemeber(string email, Guid roomId);
        bool RemoveMember(Guid userId, Guid roomId);
        bool Save();
    }
}