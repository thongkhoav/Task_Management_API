using api.Dtos;
using api.Interface;
using api.Models;
using Microsoft.EntityFrameworkCore;

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
            // assign task to user
            var task = _db.Tasks.Where(t => t.Id == Id).FirstOrDefault();
            if (task != null)
            {
                var user = _db.ApplicationUsers.Where(u => u.Id == userId).FirstOrDefault();
                if (user != null)
                {
                    task.User = user;
                    task.UserId = user.Id;
                    return Save();
                }
            }
            return false;
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

        public async Task<ICollection<TaskModel>> GetAllTaskOfRoom(Guid roomId)
        {
            // get all tasks of room and assigned user of task
            var tasks = await _db.Tasks.Where(t => t.RoomId == roomId)
            .Include(t => t.User)
            .ToListAsync();
            return tasks;
        }

        public ICollection<TaskModel> GetTasksOfRoomUser(Guid roomId, Guid userId)
        {
            throw new NotImplementedException();
        }
        public bool UpdateTask(UpdateTaskDTO task)
        {
            // update task
            var taskToUpdate = _db.Tasks.Where(t => t.Id == task.Id).FirstOrDefault();
            if (taskToUpdate != null)
            {
                taskToUpdate.Title = task.Title;
                taskToUpdate.Description = task.Description;
                taskToUpdate.DueDate = (DateTime)task.DueDate;
                taskToUpdate.IsComplete = task.IsComplete;
                taskToUpdate.UserId = task.UserId;
                return Save();
            }
            return false;

        }

        public bool Save()
        {
            var saved = _db.SaveChanges();
            return saved > 0;
        }

        public bool UpdateStatusTask(UpdateStatusTaskDTO task)
        {
            // update task status
            var taskToUpdate = _db.Tasks.Where(t => t.Id == task.Id).FirstOrDefault();
            if (taskToUpdate != null)
            {
                taskToUpdate.IsComplete = task.IsComplete;
                return Save();
            }
            return false;
        }
    }
}