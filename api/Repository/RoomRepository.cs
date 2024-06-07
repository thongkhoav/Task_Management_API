using api.Models;
using api.Interface;

namespace api.Repository
{
    public class RoomRepository : IRoomRepository
    {
        public bool AddMemeber(Guid userId, Guid roomId)
        {
            throw new NotImplementedException();
        }

        public bool CreateRoom(Guid creatorId, Room room)
        {
            throw new NotImplementedException();
        }

        public bool DeleteRoom(Guid roomId)
        {
            throw new NotImplementedException();
        }

        public ICollection<Room> GetRoombyUserId(Guid userId)
        {
            throw new NotImplementedException();
        }

        public ICollection<ApplicationUser> GetUsersOfRoom(Guid roomId)
        {
            throw new NotImplementedException();
        }

        public bool IsRoomCreator(Guid userId, Guid roomId)
        {
            throw new NotImplementedException();
        }

        public bool JoinRoom(Guid userId, Guid roomId)
        {
            throw new NotImplementedException();
        }

        public bool LeaveRoom(Guid userId, Guid roomId)
        {
            throw new NotImplementedException();
        }

        public bool Save()
        {
            throw new NotImplementedException();
        }

        public bool UpdateRoom(Room room)
        {
            throw new NotImplementedException();
        }
    }
}