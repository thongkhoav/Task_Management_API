using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Interface
{
    public interface ITaskRepository
    {
        ICollection<Task> GetAllTaskOfRoom(Guid roomId);
        ICollection<Task> GetTasksOfRoomUser(Guid roomId, Guid userId);
        bool CreateTask(Guid creatorId, Task room);
        bool UpdateTask(Task room);
        bool DeleteTask(Guid roomId);
        bool AssignTask(Guid userId, Guid roomId);
        bool Save();
    }
}