using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository userRepo;

    public AuthController(IUserRepository userRepo)
    {
        this.userRepo = userRepo;
    }

    [HttpPost("login")]
    public ActionResult<UserDto> Login(
        [FromBody] LoginRequest request)
    {
        User? user = userRepo.GetMany()
            .FirstOrDefault(u =>
                u.UserName == request.UserName &&
                u.Password == request.Password);

        if (user == null)
        {
            return Unauthorized("Invalid username or password.");
        }

        UserDto dto = new UserDto
        {
            Id = user.Id,
            UserName = user.UserName
        };

        return Ok(dto);
    }
}