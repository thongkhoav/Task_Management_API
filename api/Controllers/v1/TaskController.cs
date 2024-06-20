using System.Net;
using System.Security.Claims;
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        protected APIResponse _response;
        private readonly IMapper _mapper;
        public TaskController(
            ITaskRepository taskRepo,
            IUserRepository userRepo,
            IMapper mapper,
             IHttpContextAccessor httpContextAccessor
        )
        {
            _taskRepo = taskRepo;
            _userRepo = userRepo;
            _httpContextAccessor = httpContextAccessor;
            _response = new();
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllTaskOfRoom([FromQuery] Guid roomId)
        {
            var tasks = await _taskRepo.GetAllTaskOfRoom(roomId);
            var roomsMap = _mapper.Map<List<TaskDTO>>(tasks);

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



        // create room
        [HttpPut("assign-user")]
        [Authorize(Roles = "user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AssignTask([FromQuery] Guid taskId, [FromQuery] Guid userId)
        {
            try
            {
                // var loginUserId = "";
                // if (_httpContextAccessor.HttpContext != null)
                // {
                //     loginUserId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                // }
                // // check loginUserId is guid
                // if (!Guid.TryParse(loginUserId, out _))
                // {
                //     _response.StatusCode = HttpStatusCode.BadRequest;
                //     _response.IsSuccess = false;
                //     _response.ErrorMessages = new() { "Invalid user id" };
                //     return BadRequest(_response);
                // }

                // Guid loginUserIdd = Guid.Parse(loginUserId);
                // var isRoomMember = _userRepo.IsRoomMember(taskId, loginUserIdd);

                var isAssigned = _taskRepo.AssignTask(taskId, userId);
                if (!isAssigned)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Error while assigning task");
                    return BadRequest(_response);
                }

                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = "Assign task successfully";
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                     = new List<string>() { ex.ToString() };
            }
            return BadRequest();
        }

        [HttpPut("status")]
        [Authorize(Roles = "user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateStatusTask([FromBody] UpdateStatusTaskDTO dto)
        {
            try
            {


                // CHECK USER OF ROOM, OF TASK

                var isUpdated = _taskRepo.UpdateStatusTask(dto);
                if (!isUpdated)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Error while updating task status");
                    return BadRequest(_response);
                }
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = "Update task status successfully";
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                     = new List<string>() { ex.ToString() };
            }
            return BadRequest();
        }

        [HttpPut("{taskId}")]
        [Authorize(Roles = "user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateTask([FromRoute] Guid taskId, [FromBody] UpdateTaskDTO dto)
        {
            try
            {
                dto.Id = taskId;
                var isUpdated = _taskRepo.UpdateTask(dto);
                if (!isUpdated)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Error while updating task");
                    return BadRequest(_response);
                }
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = "Update task successfully";
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                     = new List<string>() { ex.ToString() };
            }
            return BadRequest();
        }

    }
}