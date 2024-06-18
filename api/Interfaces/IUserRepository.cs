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
        Task<ICollection<ApplicationUser>> GetUsersOfRoom(Guid roomId);

        // Task RevokeRefreshToken(TokenDTO tokenDTO);
    }
}