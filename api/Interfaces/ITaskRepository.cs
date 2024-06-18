using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Interface
{
    public interface ITaskRepository
    {
        ICollection<Task> GetAllTaskOfRoom(Guid roomId);
        ICollection<Task> GetTasksOfRoomUser(Guid roomId, Guid userId);
        bool CreateTask(TaskModel task);
        bool UpdateTask(TaskModel task);
        bool DeleteTask(Guid id);
        bool AssignTask(Guid Id, Guid userId);
        bool Save();
    }
}