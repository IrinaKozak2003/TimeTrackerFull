using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TimeTrackerServer.Dtos;
using TimeTrackerServer.Models;
using TimeTrackerServer.Service;


namespace TimeTrackerServer.Conrollers;


[ApiController]
[Route("api/v1/authenticate")]
public class AuthenticationController: ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    

    public AuthenticationController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }
    [HttpPost]
    [Route("role/create")]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest role)
    {
        var appRole = new ApplicationRole
        {
            Name = role.RoleName
        };
        await _roleManager.CreateAsync(appRole);
        return Ok(new {message = "Role created successfully"});
    }
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
    {
        AuthenticationService authenticationService = new AuthenticationService(_userManager, _roleManager);
       var result = await authenticationService.RegisterAsync(registerRequest);
       return result.Success? Ok(result) : BadRequest(result.Message);
    }

    


    [HttpPost]
    [Route("login")]
    [ProducesResponseType(typeof(LoginResponse), (int)StatusCodes.Status200OK)]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        AuthenticationService authenticationService = new AuthenticationService(_userManager, _roleManager);   
        var result = await authenticationService.LoginAsync(loginRequest);
        return result.Success? Ok(result) : BadRequest(result.Message);
        
    }
    [HttpGet]
    [Route("users")]
    [ProducesResponseType(typeof(LoginResponse), (int)StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers()
    {
        AuthenticationService authenticationService = new AuthenticationService(_userManager, _roleManager);   
        var result = await authenticationService.GetUsersAsunc();
        return result.Success? Ok(result) : BadRequest(result.Message);
        
    }

    [HttpGet]
    [Authorize]
    [Route("user/{id}")]
    public async Task<IActionResult> GetUserById(string id)
    {
        AuthenticationService authenticationService = new AuthenticationService(_userManager, _roleManager);   
        var result = await authenticationService.GetUserById(id);
        return  Ok(result) ;

    }
    [HttpGet]
    [Authorize]
    [Route("users/all")]
    public async Task<IActionResult> GetAllUsers()
    {
        AuthenticationService authenticationService = new AuthenticationService(_userManager, _roleManager);   
        var result = await authenticationService.GetAllUsersAsunc();
        return result.Success? Ok(result) : BadRequest(result.Message);
        
    }
    [HttpPut("update")]
    [Authorize]
    public async Task<IActionResult> UpdateUser(UpdateUserRequest updateUserRequest)
    {
        AuthenticationService authenticationService = new AuthenticationService(_userManager, _roleManager); 
        var result = await authenticationService.UpdateUserAsync(updateUserRequest);
        if (result.Success)
        {
            return Ok(result);
        }
        else
        {
            return BadRequest(result);
        }
    }

    [HttpDelete("delete/{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateUser(string id)
    {
        AuthenticationService authenticationService = new AuthenticationService(_userManager, _roleManager); 
        var result = await authenticationService.DeleteUserAsync(id);
        if (result.Success)
        {
            return Ok(result);
        }
        else
        {
            return BadRequest(result);
        }
    }
}

