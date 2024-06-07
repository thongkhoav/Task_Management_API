using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.Dtos.Account;

namespace api.Interface
{
    public interface IUserRepository
    {
        bool IsUniqueUser(string email);
        Task<TokenDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<UserDTO> Register(RegisterationRequestDTO registerationRequestDTO);
        Task<TokenDTO> RefreshAccessToken(TokenDTO tokenDTO);

        // Task RevokeRefreshToken(TokenDTO tokenDTO);
    }
}