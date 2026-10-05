using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository userRepo;

    public UsersController(IUserRepository userRepo)
    {
        this.userRepo = userRepo;
    }

    // CREATE USER
    [HttpPost]
    public async Task<ActionResult<UserDto>> AddUser(
        [FromBody] CreateUserDto request)
    {
        // Check if username already exists
        bool usernameTaken = userRepo
            .GetMany()
            .Any(u => u.UserName == request.UserName);

        if (usernameTaken)
        {
            return BadRequest("Username is already taken.");
        }

        User user = new User
        {
            UserName = request.UserName,
            Password = request.Password
        };

        User created = await userRepo.AddAsync(user);

        UserDto dto = new UserDto
        {
            Id = created.Id,
            UserName = created.UserName
        };

        return Created($"/Users/{dto.Id}", dto);
    }


    // GET ALL USERS + FILTER BY USERNAME
    [HttpGet]
    public ActionResult<IEnumerable<UserDto>> GetMany(
        [FromQuery] string? userName)
    {
        IQueryable<User> users = userRepo.GetMany();

        if (!string.IsNullOrWhiteSpace(userName))
        {
            users = users.Where(u =>
                u.UserName.Contains(userName));
        }

        List<UserDto> result = users
            .Select(u => new UserDto
            {
                Id = u.Id,
                UserName = u.UserName
            })
            .ToList();

        return Ok(result);
    }


    // GET ONE USER
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetSingle(int id)
    {
        try
        {
            User user = await userRepo.GetSingleAsync(id);

            UserDto dto = new UserDto
            {
                Id = user.Id,
                UserName = user.UserName
            };

            return Ok(dto);
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }


    // UPDATE USER
    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(
        int id,
        [FromBody] CreateUserDto request)
    {
        try
        {
            User user = new User
            {
                Id = id,
                UserName = request.UserName,
                Password = request.Password
            };

            await userRepo.UpdateAsync(user);

            return NoContent();
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }


    // DELETE USER
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await userRepo.DeleteAsync(id);

            return NoContent();
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }
}