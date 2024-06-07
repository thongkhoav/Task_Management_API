using api.Interface;

namespace api.Repository
{
    public class TaskRepository : ITaskRepository
    {
        public bool AssignTask(Guid userId, Guid roomId)
        {
            throw new NotImplementedException();
        }

        public bool CreateTask(Guid creatorId, Task room)
        {
            throw new NotImplementedException();
        }

        public bool DeleteTask(Guid roomId)
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
            throw new NotImplementedException();
        }

        public bool UpdateTask(Task room)
        {
            throw new NotImplementedException();
        }
    }
}