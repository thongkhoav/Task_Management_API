using System.Net;
using api.Dtos;
using api.Dtos.Api;
using api.Interface;
using api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/tasks")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskRepository _taskRepo;
        private readonly IUserRepository _userRepo;
        protected APIResponse _response;
        private readonly IMapper _mapper;
        public TaskController(
            ITaskRepository taskRepo,
            IUserRepository userRepo,
            IMapper mapper
        )
        {
            _taskRepo = taskRepo;
            _userRepo = userRepo;
            _response = new();
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllTaskOfRoom([FromBody]
         CreateTaskDTO createTaskDTO
        )
        {
            var rooms = _taskRepo.GetAllTaskOfRoom(createTaskDTO.RoomId);
            var roomsMap = _mapper.Map<List<RoomDTO>>(rooms);

            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = roomsMap;
            return Ok(_response);
        }

        // create room
        [HttpPost]
        [Authorize(Roles = "user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDTO taskDTO)
        {
            try
            {
                Console.WriteLine("Error21123: " + taskDTO);
                if (!ModelState.IsValid)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage)).ToList();
                    Console.WriteLine("Error: " + _response.ErrorMessages);
                    return BadRequest(_response);
                }

                Console.WriteLine("TaskDTO: " + taskDTO.DueDate);
                // var newTaskDTO = new CreateTaskDTO()
                // {
                //     RoomId = taskDTO.RoomId,
                //     UserId = taskDTO.UserId,
                //     Title = "giat do 2",
                //     Description = "giat do 2 23132",
                //     DueDate = ,
                // };
                var task = _mapper.Map<TaskModel>(taskDTO);
                Console.WriteLine("Task23: " + task);
                var isCreatedTask = _taskRepo.CreateTask(task);
                if (!isCreatedTask)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Error while creating task");
                    return BadRequest(_response);
                }
                Console.WriteLine("Task created123213");

                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = "Task created successfully";
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                     = new List<string>() { ex.ToString() };
            }
            Console.WriteLine("Errorasdsad: " + _response.ErrorMessages);
            return BadRequest();
        }
    }
}