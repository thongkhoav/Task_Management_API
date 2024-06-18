using api.Interface;
using api.Models;

namespace api.Repository
{
    public class TaskRepository : ITaskRepository
    {
        private ApplicationDbContext _db;
        public TaskRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public bool AssignTask(Guid Id, Guid userId)
        {
            throw new NotImplementedException();
        }

        public bool CreateTask(TaskModel task)
        {
            Console.WriteLine("Creating task");
            if (task.UserId != null)
            {
                var assignUser = _db.ApplicationUsers.Where(a => a.Id == task.UserId).FirstOrDefault();
                if (assignUser != null)
                {
                    task.User = assignUser;
                    task.UserId = assignUser.Id;
                }
            }
            var room = _db.Rooms.Where(a => a.Id ==
            task.RoomId).FirstOrDefault();
            if (room != null)
            {
                task.Room = room;
                task.RoomId = room.Id;
            }
            task.IsComplete = false;
            _db.Tasks.Add(task);
            Console.WriteLine("Task created");
            return Save();
        }

        public bool DeleteTask(Guid id)
        {
            throw new NotImplementedException();
        }

        public ICollection<Task> GetAllTaskOfRoom(Guid roomId)
        {
            throw new NotImplementedException();
        }

        public ICollection<Task> GetTasksOfRoomUser(Guid roomId, Guid userId)
        {
            throw new NotImplementedException();
        }

        public bool Save()
        {
            var saved = _db.SaveChanges();
            return saved > 0;
        }

        public bool UpdateTask(TaskModel task)
        {
            throw new NotImplementedException();
        }
    }
}