using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Interface
{
    public interface IRoomRepository
    {
        ICollection<Room> GetRoomsByUserId(Guid userId);
        Task<ICollection<ApplicationUser>> GetUsersOfRoom(Guid roomId);
        bool IsRoomCreator(Guid userId, Guid roomId); // check if user is room
        bool CreateRoom(Guid creatorId, Room room);
        bool UpdateRoom(Room room);
        bool DeleteRoom(Guid roomId);
        bool LeaveRoom(Guid userId, Guid roomId);
        bool JoinRoom(Guid userId, Guid roomId);
        bool AddMemeber(Guid userId, Guid roomId);
        bool Save();
    }
}