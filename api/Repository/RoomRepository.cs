using api.Models;
using api.Interface;

namespace api.Repository
{
    public class RoomRepository : IRoomRepository
    {
        private ApplicationDbContext _db;
        public RoomRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public bool AddMemeber(Guid userId, Guid roomId)
        {
            throw new NotImplementedException();
        }

        public bool CreateRoom(Guid creatorId, Room room)
        {
            var creator = _db.ApplicationUsers.Where(a => a.Id == creatorId).FirstOrDefault();

            var userRoom = new UserRoom()
            {
                User = creator,
                Room = room,
                RoomId = room.Id,
                UserId = creatorId
            };

            _db.UserRooms.Add(userRoom);
            _db.Rooms.Add(room);

            return Save();
        }

        public bool DeleteRoom(Guid roomId)
        {
            throw new NotImplementedException();
        }

        public ICollection<Room?> GetRoomsByUserId(Guid userId)
        {
            // get all rooms of user
            var rooms = _db.UserRooms.Where(u => u.UserId == userId && u.Room != null)
            .OrderByDescending(ur => ur.Room!.CreatedAt)
            .Select(r => r.Room).ToList();
            return rooms;
        }



        public bool IsRoomCreator(Guid userId, Guid roomId)
        {
            // check if user is room creator
            var room = _db.UserRooms.Where(r => r.RoomId == roomId && r.UserId == userId && r.IsOwner).FirstOrDefault();
            return room != null;
        }

        public bool IsRoomExist(Guid roomId)
        {
            // check room exist
            var room = _db.Rooms.Where(r => r.Id == roomId).FirstOrDefault();
            return room != null;
        }

        public bool IsRoomMember(Guid roomId, Guid userId)
        {
            var room = _db.UserRooms.Where(r => r.RoomId == roomId && r.UserId == userId).FirstOrDefault();
            return room != null;
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
            var saved = _db.SaveChanges();
            return saved > 0;
        }

        public bool UpdateRoom(Room room)
        {
            throw new NotImplementedException();
        }


    }
}