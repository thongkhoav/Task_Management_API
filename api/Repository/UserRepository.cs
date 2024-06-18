using AutoMapper;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using api.Dtos;
using Microsoft.EntityFrameworkCore;
using api.Dtos.Account;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using api.Interface;

namespace api.Repository
{
    public class UserRepository : IUserRepository
    {
        private ApplicationDbContext _db;
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole<Guid>> _roleManager;
        private IMapper _mapper;
        private string secretKey;
        private int tokenExpire;
        private IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserRepository(
            ApplicationDbContext db,
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
            Console.WriteLine("secretKey56465: " + configuration.GetSection("ApiSettings:Secret").Value);
            _configuration = configuration;
            tokenExpire = configuration.GetValue<int>("ApiSettings:AccessTokenExpirationMinutes");
            _httpContextAccessor = httpContextAccessor;
        }

        public bool IsUniqueUser(string email)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == email);
            return user == null;
        }

        public async Task<TokenDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            Console.WriteLine("LoginRequestDTO: " + loginRequestDTO.Email);
            var user = _db.Users.FirstOrDefault(u => u.Email == loginRequestDTO.Email);
            Console.WriteLine("Userasdad: " + user);
            if (user == null)
            {
                Console.WriteLine("User not found");
                return new TokenDTO()
                {
                    AccessToken = ""
                };
            }
            bool isValidPassword = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);
            if (!isValidPassword)
            {
                Console.WriteLine("Invalid password");
                return new TokenDTO()
                {
                    AccessToken = ""
                };
            }
            var jwtTokenId = $"JTI{Guid.NewGuid()}";
            Console.WriteLine("jwtTokenId: " + jwtTokenId);
            Console.WriteLine("User123123 " + user.PasswordHash);
            var accessToken = await CreateAccessToken(user, jwtTokenId);
            var refreshToken = await CreateNewRefreshToken(user.Id.ToString(), jwtTokenId);
            return new TokenDTO()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        private async Task<string> CreateAccessToken(ApplicationUser user, string jwtTokenId)
        {
            Console.WriteLine("CreateAccessToken: " + user.Email);

            var roles = await _userManager.GetRolesAsync(user);
            Console.WriteLine("secretKeysasd: " + secretKey);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            Console.WriteLine("secretKey: " + secretKey);
            Console.WriteLine("tokenExpire: " + tokenExpire);
            Console.WriteLine("useridsadsd: " + user.Id.ToString());
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                    new Claim(JwtRegisteredClaimNames.Jti, jwtTokenId),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                }),
                Expires = DateTime.UtcNow.AddMinutes(tokenExpire),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private async Task<string> CreateNewRefreshToken(string userId, string tokenId)
        {
            // create and save refresh token to the DATABASE
            var refreshToken = new RefreshToken
            {
                UserId = userId,
                JwtTokenId = tokenId,
                Refresh_Token = Guid.NewGuid().ToString(),
                IsValid = true,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5)
            };
            _db.RefreshTokens.Add(refreshToken);
            await _db.SaveChangesAsync();
            return refreshToken.Refresh_Token;
        }

        public Task<TokenDTO> RefreshAccessToken(TokenDTO tokenDTO)
        {
            throw new NotImplementedException();
        }

        public async Task<UserDTO> Register(RegisterationRequestDTO registerationRequestDTO)
        {
            ApplicationUser user = new()
            {
                Email = registerationRequestDTO.Email,
                Name = registerationRequestDTO.Name,
                UserName = registerationRequestDTO.Email
            };
            try
            {
                var result = await _userManager.CreateAsync(user, registerationRequestDTO.Password);
                Console.WriteLine("asda1212312321: " + result);
                if (result.Succeeded)
                {
                    Console.WriteLine("asdassdasd: " + registerationRequestDTO.Role);
                    if (!_roleManager.RoleExistsAsync(registerationRequestDTO.Role).GetAwaiter().GetResult())
                    {
                        await _roleManager.CreateAsync(new IdentityRole<Guid>(registerationRequestDTO.Role));
                    }
                    await _userManager.AddToRoleAsync(user, registerationRequestDTO.Role);
                    var userReturn = _db.Users.FirstOrDefault(u => u.Email == user.Email);
                    return _mapper.Map<UserDTO>(userReturn);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return new UserDTO();
        }

        public async Task<UserDTO?> GetUserFromToken()
        {
            var userId = string.Empty;
            if (_httpContextAccessor.HttpContext != null)
            {
                userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
            var userMap = _mapper.Map<UserDTO>(user);
            return userMap;
        }

        // public async Task RevokeRefreshToken(TokenDTO tokenDTO)
        // {
        //     throw new NotImplementedException();
        // }

        public async Task<ICollection<ApplicationUser>> GetUsersOfRoom(Guid roomId)
        {
            throw new NotImplementedException();
        }
    }
}