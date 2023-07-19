using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using TimeTrackerServer.Dtos;
using TimeTrackerServer.Models;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace TimeTrackerServer.Service;

public class AuthenticationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public AuthenticationService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<RegisterResponce> RegisterAsync(RegisterRequest registerRequest)
    {
        try
        {
            var userExists = await _userManager.FindByEmailAsync(registerRequest.Email);
            if (userExists != null)
                return new RegisterResponce
                {
                    Success = false,
                    Message = "User already exists"
                };
            userExists = new ApplicationUser
            {
                UserName = registerRequest.Username,
                Email = registerRequest.Email,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                PackageIds = new List<ObjectId>(),
                Cycles = new List<Cycle>()
            };
            var createUserResult = await _userManager.CreateAsync(userExists, registerRequest.Password);
            if (!createUserResult.Succeeded)
                return new RegisterResponce
                {
                    Success = false,
                    Message = createUserResult.Errors.First().Description
                };
            var addUsertoRoleResult = await _userManager.AddToRoleAsync(userExists, registerRequest.Role);
            if (!addUsertoRoleResult.Succeeded)
                return new RegisterResponce
                {
                    Success = false,
                    Message = addUsertoRoleResult.Errors.First().Description
                };
            return new RegisterResponce
            {
                Success = true,
                Message = "User created successfully"
            };


        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return new RegisterResponce
            {
                Success = false,
                Message = e.Message
            };
        }
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(loginRequest.Email);
            if (user == null)
                return new LoginResponse
                {
                    Success = false,
                    Message = "User not found"
                };

            var passwordValid = await _userManager.CheckPasswordAsync(user, loginRequest.Password);
            if (!passwordValid)
                return new LoginResponse
                {
                    Success = false,
                    Message = "Invalid password"
                };
            

            // Password is valid, proceed with generating the JWT token

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role));
            claims.AddRange(roleClaims);


            // Key from appsettings.json
            var signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("KEY1312312312RANDO@SDKFSDJNWOERE"));
            var creds = new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(30);

            var token = new JwtSecurityToken(
                issuer: "https://localhost:44301",
                audience: "http://localhost:3000",
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new LoginResponse
            {
                Success = true,
                Message = "Login successful",
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                Email = user.Email,
                UserId = user.Id.ToString(),
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return new LoginResponse
            {
                Success = false,
                Message = e.Message
            };
        }
    }




    public async Task<UserResponse> GetUsersAsunc()
    {
        try
        {
            var MongoUsers = await _userManager.GetUsersInRoleAsync("User");
            List<User> users = new List<User>();
            foreach (var i in MongoUsers)
            {
                
                User user = new User
                {
                    UserId = i.Id.ToString(),
                    UserName = i.UserName
                };
                users.Add(user);

            }

            return new UserResponse
            {
                Users = users,
                Success = true
            };
        }
        catch (Exception ex)
        {
            return new UserResponse
            {
                Users = null,
                Success = false,
                Message = "Get users error"
            };  
        }
        
    }

    public async Task<ApplicationUser> GetUserById(string id)
    { return await _userManager.FindByIdAsync(id);
         
    }
    public async Task<UserResponse> GetAllUsersAsunc()
    {
        try
        {
            var MongoUsers = _userManager.Users;
            List<User> users = new List<User>();
            foreach (var i in MongoUsers)
            {
                
                User user = new User
                {
                    UserId = i.Id.ToString(),
                    UserName = i.UserName
                };
                users.Add(user);

            }

            return new UserResponse
            {
                Users = users,
                Success = true
            };
        }
        catch (Exception ex)
        {
            return new UserResponse
            {
                Users = null,
                Success = false,
                Message = "Get users error"
            };  
        }
        
    }
    public async Task<UpdateUserResponse> UpdateUserAsync(UpdateUserRequest updateUserRequest)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(updateUserRequest.UserId);
            if (user == null)
            {
                return new UpdateUserResponse
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            // Обновляем необходимые свойства пользователя
            user.UserName = updateUserRequest.UserName;

            // Сохраняем изменения в базе данных
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return new UpdateUserResponse
                {
                    Success = false,
                    Message = updateResult.Errors.First().Description
                };
            }

            return new UpdateUserResponse
            {
                Success = true,
                Message = "User updated successfully"
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return new UpdateUserResponse
            {
                Success = false,
                Message = ex.Message
            };
        }
    }
    public async Task<UpdateUserResponse> DeleteUserAsync(string id)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id);
        
            if (user == null)
            {
                // Handle the case when the user is not found
                return new UpdateUserResponse { Success = false, Message = "User not found" };
            }
        
            var result = await _userManager.DeleteAsync(user);
        
            if (result.Succeeded)
            {
                // User deletion succeeded
                return new UpdateUserResponse { Success = true, Message = "User deleted successfully" };
            }
            else
            {
                // User deletion failed
                return new UpdateUserResponse { Success = false, Message = "User deletion failed" };
            }
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as per your application's requirements
            return new UpdateUserResponse { Success = false, Message = "An error occurred while deleting the user" };
        }
    }


}