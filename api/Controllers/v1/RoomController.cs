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
    [Route("api/v1/rooms")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomRepository _roomRepo;
        private readonly IUserRepository _userRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        protected APIResponse _response;
        private readonly IMapper _mapper;
        public RoomController(
            IRoomRepository roomRepo,
            IUserRepository userRepo,
            IMapper mapper,
             IHttpContextAccessor httpContextAccessor
        )
        {
            _roomRepo = roomRepo;
            _userRepo = userRepo;
            _httpContextAccessor = httpContextAccessor;
            _response = new();
            _mapper = mapper;
        }

        [HttpGet("current-user")]
        [Authorize(Roles = "user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRoomsOfUser()
        {
            var currentUser = await _userRepo.GetUserFromToken();
            if (currentUser == null)
            {
                _response.StatusCode = HttpStatusCode.Unauthorized;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Unauthorized");
                return Unauthorized(_response);
            }
            var rooms = _roomRepo.GetRoomsByUserId(currentUser.Id);
            // var roomsMap = _mapper.Map<List<RoomDTO>>(rooms);

            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = rooms;
            return Ok(_response);
        }

        [HttpGet]
        [Authorize(Roles = "user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRooms()
        {
            var currentUser = await _userRepo.GetUserFromToken();
            if (currentUser == null)
            {
                _response.StatusCode = HttpStatusCode.Unauthorized;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Unauthorized");
                return Unauthorized(_response);
            }
            var rooms = _roomRepo.GetAllRoom(currentUser.Id);
            // var roomsMap = _mapper.Map<List<RoomDTO>>(rooms);
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = rooms;
            return Ok(_response);
        }

        [HttpGet("{roomId}")]
        [Authorize(Roles = "user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetRoomDetail(Guid roomId)
        {
            var room = _roomRepo.GetRoomById(roomId);
            if (room == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Room not found");
                return NotFound(_response);
            }
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = room;
            return Ok(_response);
        }

        // create room
        [HttpPost]
        [Authorize(Roles = "user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateRoom([FromBody] CreateRoomDTO roomDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage)).ToList();
                    return BadRequest(_response);
                }

                var currentUser = await _userRepo.GetUserFromToken();
                if (currentUser == null)
                {
                    _response.StatusCode = HttpStatusCode.Unauthorized;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Unauthorized");
                    return Unauthorized(_response);
                }

                var room = _mapper.Map<Room>(roomDTO);

                var isCreatedRoom = _roomRepo.CreateRoom(currentUser.Id, room);
                if (!isCreatedRoom)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Error while creating room");
                    return BadRequest(_response);
                }

                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = "Room created successfully";
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

        [HttpPost("member")]
        [Authorize(Roles = "user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult AddMember([FromBody] AddMemberDTO obj)
        {
            var userId = "";
            if (_httpContextAccessor.HttpContext != null)
            {
                userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            Console.WriteLine("userId: " + userId);
            // check userId is guid
            if (!Guid.TryParse(userId, out _))
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages = new() { "Invalid user id" };
                return BadRequest(_response);
            }
            Guid userIdd = Guid.Parse(userId);
            var IsRoomMember = _roomRepo.IsRoomMember(obj.RoomId, obj.Email);
            if (IsRoomMember)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("User is already in room");
                return BadRequest(_response);
            }
            var IsRoomOwner = _roomRepo.IsRoomCreator(userIdd, obj.RoomId);
            if (!IsRoomOwner)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("You are not room leader");
                return BadRequest(_response);
            }

            var isAdded = _roomRepo.AddMemeber(obj.Email, obj.RoomId);
            if (!isAdded)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Error while adding member");
                return BadRequest(_response);
            }

            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = "Member added successfully";
            return Ok(_response);
        }

        [HttpDelete("member")]
        [Authorize(Roles = "user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult RemoveMember([FromBody] RemoveMemberDTO obj)
        {
            var userId = "";
            if (_httpContextAccessor.HttpContext != null)
            {
                userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            Console.WriteLine("userId: " + userId);
            // check userId is guid
            if (!Guid.TryParse(userId, out _))
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages = new() { "Invalid user id" };
                return BadRequest(_response);
            }
            Guid userIdd = Guid.Parse(userId);
            var IsRoomMember = _roomRepo.IsRoomMember(obj.UserId, obj.RoomId);
            if (!IsRoomMember)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("User are not room member");
                return BadRequest(_response);
            }

            var IsRoomOwner = _roomRepo.IsRoomCreator(userIdd, obj.RoomId);
            if (!IsRoomOwner)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("You are not room leader");
                return BadRequest(_response);
            }

            var isRemoved = _roomRepo.RemoveMember(obj.UserId, obj.RoomId);
            if (!isRemoved)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Error while removing member");
                return BadRequest(_response);
            }

            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = "Member removed successfully";
            return Ok(_response);
        }
    }
}