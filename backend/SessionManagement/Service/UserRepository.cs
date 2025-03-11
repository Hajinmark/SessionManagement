using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SessionManagement.Data;
using SessionManagement.Interface;
using SessionManagement.Models;
using SessionManagement.Models.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SessionManagement.Service
{
    public class UserRepository : IUser
    {
        private readonly LoginDbContext dbContext;
        private readonly PasswordHasher<Object> passwordHasher = new PasswordHasher<object>();
        private readonly IConfiguration _config;
        public UserRepository(LoginDbContext dbContext, IConfiguration _config)
        {
            this.dbContext = dbContext;
            this._config = _config;
        }

        public async Task<AuthenticationDTO> LoginUser(UserLogin user)
        {
            var userLogin = await dbContext.Users
                                .FirstOrDefaultAsync(x => x.Username == user.Username);

            var verifyPassword = passwordHasher.VerifyHashedPassword(null, userLogin?.PasswordHash, user.Password);

            try
            {
                // User does not exist
                if (userLogin == null)
                    return new AuthenticationDTO { Message = "User does not exist", IsLogin = false};

                // Password does not match
                if (verifyPassword != PasswordVerificationResult.Success)
                    return new AuthenticationDTO { Message = "Password does not match",IsLogin = false };

                // Successfully Login
                else
                {
                    var token = GenerateToken(user);
                    return new AuthenticationDTO { Message = "Successfully Login", Token = token.Token, IsLogin = true, Expiration = token.Expiration};
                }
               

            }

            catch(Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }

        public async Task<bool> RegisterUser(AddUser user)
        {
          
            var isUserExist = await dbContext.Users
                             .AnyAsync(x => x.Username == user.Username);
            try
            {
                if(!isUserExist)
                {
                    var addUser = new Users()
                    {
                        Username = user.Username,
                        Email = user.Email,
                        PasswordHash = passwordHasher.HashPassword(null, user.PasswordHash),
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsActive = user.IsActive,
                    };

                    dbContext.Users.Add(addUser);   
                    await dbContext.SaveChangesAsync();

                    var addDetails = new UserDetails()
                    {
                        UserID = addUser.UserID,
                        FirstName = user.FirstName,
                        LastName = user.LastName
                    };

                    dbContext.UserDetails.Add(addDetails);
                    await dbContext.SaveChangesAsync();

                    // Successfully add new user
                    return true;
                }

                return false;
            }

            catch(Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }

        private AuthenticationDTO GenerateToken(UserLogin user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.Username)
            };

            var expiration = DateTime.UtcNow.AddMinutes(1);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: expiration,
                signingCredentials: credentials);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new AuthenticationDTO { Token = tokenString, Expiration = expiration };
        }

    }
}
