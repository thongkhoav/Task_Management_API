using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.Models;

namespace api.Interface
{
    public interface ITaskRepository
    {
        Task<ICollection<TaskModel>> GetAllTaskOfRoom(Guid roomId);
        ICollection<TaskModel> GetTasksOfRoomUser(Guid roomId, Guid userId);
        bool CreateTask(TaskModel task);
        bool UpdateTask(UpdateTaskDTO task);
        bool DeleteTask(Guid id);
        bool AssignTask(Guid Id, Guid userId);
        bool UpdateStatusTask(UpdateStatusTaskDTO task);
        bool Save();
    }
}