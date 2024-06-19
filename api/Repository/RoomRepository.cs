using api.Models;
using api.Interface;
using Microsoft.EntityFrameworkCore;
using api.Dtos.Room;
using api.Dtos;
using AutoMapper;

namespace api.Repository
{
    public class RoomRepository : IRoomRepository
    {
        private ApplicationDbContext _db;
        private IMapper _mapper;
        public RoomRepository(ApplicationDbContext db,
            IMapper mapper
        )
        {
            _db = db;
            _mapper = mapper;
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
            userRoom.IsOwner = true;

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

        public ICollection<RoomDTO?> GetAllRoom(Guid userId)
        {
            // IsJoined =  
            var rooms = _db.Rooms
                .OrderByDescending(ur => ur.CreatedAt)
                .Select(ur => new RoomDTO
                {
                    Id = ur!.Id,
                    Name = ur!.Name,
                    Description = ur!.Description,
                    IsJoined = _db.UserRooms.Any(
                        u => u.UserId == userId && u.RoomId == ur.Id
                    )
                }).ToList();
            return rooms;
        }

        public RoomWithOwnerDTO? GetRoomById(Guid roomId)
        {


            var user = _db.UserRooms
                    .Where(ur => ur.IsOwner && ur.RoomId == roomId)
                    .Select(r => r.User)
                    .FirstOrDefault();
            var room = _db.Rooms
                    .Where(r => r.Id == roomId)
                    .FirstOrDefault();

            var roomWithOwner = new RoomWithOwnerDTO()
            {
                RoomName = room.Name ?? "",
                RoomDescription = room.Description,
                Owner = _mapper.Map<UserDTO>(user)
            };

            return roomWithOwner;
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