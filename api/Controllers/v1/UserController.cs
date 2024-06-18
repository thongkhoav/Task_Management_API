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
    [Route("api/v1/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly IRoomRepository _roomRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        protected APIResponse _response;
        private readonly IMapper _mapper;
        public UserController(
            IUserRepository userRepo,
            IRoomRepository roomRepo,
            IMapper mapper,
             IHttpContextAccessor httpContextAccessor
        )
        {
            _userRepo = userRepo;
            _roomRepo = roomRepo;
            _httpContextAccessor = httpContextAccessor;
            _response = new();
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsersOfRoom([FromQuery] Guid roomId, [FromQuery] bool includeOwner = false)
        {
            var users = await _userRepo.GetUsersOfRoom(roomId, includeOwner);
            var usersMap = _mapper.Map<List<UserDTO>>(users);
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = usersMap;
            return Ok(_response);
        }

        [HttpPost("join-room")]
        [Authorize(Roles = "user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult JoinRoom([FromBody] JoinRoomDTO obj)
        {
            var userId = "";
            if (_httpContextAccessor.HttpContext != null)
            {
                userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            // check userId is guid
            if (!Guid.TryParse(userId, out _))
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages = new() { "Invalid user id" };
                return BadRequest(_response);
            }

            Guid userIdd = Guid.Parse(userId);
            var users = _userRepo.JoinRoom(userIdd, obj.RoomId);
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = "Joined room successfully";
            return Ok(_response);
        }
    }
}