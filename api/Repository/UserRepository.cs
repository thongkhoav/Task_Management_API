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

        public async Task Logout(TokenDTO tokenDTO)
        {
            var existingRefreshToken = await _db.RefreshTokens.FirstOrDefaultAsync(_ => _.Refresh_Token == tokenDTO.RefreshToken);

            if (existingRefreshToken == null)
                return;

            // Compare data from existing refresh and access token provided and
            // if there is any missmatch then we should do nothing with refresh token

            var isTokenValid = CheckValidAccessToken(tokenDTO.AccessToken, existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);
            if (!isTokenValid)
            {
                return;
            }

            await MarkAllTokenInChainAsInvalid(existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);
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
                    new Claim(JwtRegisteredClaimNames.Sub , user.Id.ToString()),
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
                Refresh_Token = Guid.NewGuid() + "-" + Guid.NewGuid(),
                IsValid = true,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5)
            };
            await _db.RefreshTokens.AddAsync(refreshToken);
            await _db.SaveChangesAsync();
            return refreshToken.Refresh_Token;
        }

        private Task MarkTokenAsInvalid(RefreshToken refreshToken)
        {
            refreshToken.IsValid = false;
            return _db.SaveChangesAsync();
        }

        private async Task MarkAllTokenInChainAsInvalid(string userId, string tokenId)
        {
            await _db.RefreshTokens.Where(u => u.UserId == userId
               && u.JwtTokenId == tokenId)
                   .ExecuteUpdateAsync(u => u.SetProperty(refreshToken => refreshToken.IsValid, false));

        }

        public async Task<TokenDTO> RefreshAccessToken(TokenDTO tokenDTO)
        {
            // Find an existing refresh token
            var existingRefreshToken = await _db.RefreshTokens.FirstOrDefaultAsync(u => u.Refresh_Token == tokenDTO.RefreshToken);
            if (existingRefreshToken == null)
            {
                return new TokenDTO();
            }

            var isTokenValid = CheckValidAccessToken(tokenDTO.AccessToken, existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);
            if (!isTokenValid)
            {
                await MarkTokenAsInvalid(existingRefreshToken);
                return new TokenDTO();
            }

            if (!existingRefreshToken.IsValid)
            {
                await MarkAllTokenInChainAsInvalid(existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);
            }

            if (existingRefreshToken.ExpiresAt < DateTime.UtcNow)
            {
                await MarkTokenAsInvalid(existingRefreshToken);
                return new TokenDTO();
            }

            var newRefreshToken = await CreateNewRefreshToken(existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);
            await MarkTokenAsInvalid(existingRefreshToken);

            var applicationUser = _db.ApplicationUsers.FirstOrDefault(u => u.Id.ToString() == existingRefreshToken.UserId);
            if (applicationUser == null)
                return new TokenDTO();

            var newAccessToken = await CreateAccessToken(applicationUser, existingRefreshToken.JwtTokenId);

            return new TokenDTO()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
            };

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

        private bool CheckValidAccessToken(string accessToken, string expectedUserId, string expectedTokenId)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwt = tokenHandler.ReadJwtToken(accessToken);
                var jwtTokenId = jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Jti).Value;
                var userId = jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value;

                return userId == expectedUserId && jwtTokenId == expectedTokenId;
            }
            catch
            {
                return false;
            }
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

        public async Task<ICollection<ApplicationUser>> GetUsersOfRoom(Guid roomId, bool IncludeOwner)
        {
            var users = await _db.UserRooms
                .Where(ur => ur.RoomId == roomId && (IncludeOwner || !ur.IsOwner))
                .Select(u => u.User)
                .ToListAsync();
            return users;
        }

        public bool JoinRoom(Guid userId, Guid roomId)
        {
            var user = _db.ApplicationUsers.Where(a => a.Id == userId).FirstOrDefault();
            var room = _db.Rooms.Where(r => r.Id == roomId).FirstOrDefault();
            var userRoom = new UserRoom()
            {
                User = user,
                Room = room,
                RoomId = roomId,
                UserId = userId
            };
            userRoom.IsOwner = false;
            _db.UserRooms.Add(userRoom);
            return Save();
        }

        public bool LeaveRoom(Guid userId, Guid roomId)
        {
            throw new NotImplementedException();
        }

        public bool Save()
        {
            var saved = _db.SaveChanges();
            return saved > 0;
        }


    }
}