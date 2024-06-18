using api.Dtos;
using api.Dtos.Account;
using api.Models;

namespace api.Interface
{
    public interface IUserRepository
    {
        bool IsUniqueUser(string email);
        Task<TokenDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<UserDTO> Register(RegisterationRequestDTO registerationRequestDTO);
        Task<TokenDTO> RefreshAccessToken(TokenDTO tokenDTO);
        Task<UserDTO?> GetUserFromToken();
        Task<ICollection<ApplicationUser>> GetUsersOfRoom(Guid roomId, bool IncludeOwner);
        bool LeaveRoom(Guid userId, Guid roomId);
        bool JoinRoom(Guid userId, Guid roomId);
        bool Save();

        // Task RevokeRefreshToken(TokenDTO tokenDTO);
    }
}