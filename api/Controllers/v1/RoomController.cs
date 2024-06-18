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
    [Route("api/v1/rooms")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomRepository _roomRepo;
        private readonly IUserRepository _userRepo;
        protected APIResponse _response;
        private readonly IMapper _mapper;
        public RoomController(
            IRoomRepository roomRepo,
            IUserRepository userRepo,
            IMapper mapper
        )
        {
            _roomRepo = roomRepo;
            _userRepo = userRepo;
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


    }
}