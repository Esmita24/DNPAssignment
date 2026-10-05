using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostRepository postRepo;
    private readonly IUserRepository userRepo;

    public PostsController(
        IPostRepository postRepo,
        IUserRepository userRepo)
    {
        this.postRepo = postRepo;
        this.userRepo = userRepo;
    }

    // CREATE POST
    [HttpPost]
    public async Task<ActionResult<Post>> AddPost(
        [FromBody] Post post)
    {
        bool userExists = userRepo
            .GetMany()
            .Any(u => u.Id == post.UserId);

        if (!userExists)
        {
            return BadRequest("User does not exist.");
        }

        Post created = await postRepo.AddAsync(post);

        return Created($"/Posts/{created.Id}", created);
    }

    // GET ALL POSTS + FILTER
    [HttpGet]
    public ActionResult<IEnumerable<Post>> GetMany(
        [FromQuery] string? title,
        [FromQuery] int? userId,
        [FromQuery] string? userName)
    {
        IQueryable<Post> posts = postRepo.GetMany();

        // Filter by title
        if (!string.IsNullOrWhiteSpace(title))
        {
            posts = posts.Where(p =>
                p.Title.Contains(title));
        }

        // Filter by user ID
        if (userId.HasValue)
        {
            posts = posts.Where(p =>
                p.UserId == userId.Value);
        }

        // Filter by username
        if (!string.IsNullOrWhiteSpace(userName))
        {
            User? user = userRepo
                .GetMany()
                .FirstOrDefault(u =>
                    u.UserName.Contains(userName));

            if (user is null)
            {
                return Ok(new List<Post>());
            }

            posts = posts.Where(p =>
                p.UserId == user.Id);
        }

        return Ok(posts.ToList());
    }

    // GET ONE POST
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Post>> GetSingle(int id)
    {
        try
        {
            Post post = await postRepo.GetSingleAsync(id);

            return Ok(post);
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }

    // UPDATE POST
    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(
        int id,
        [FromBody] Post post)
    {
        try
        {
            post.Id = id;

            bool userExists = userRepo
                .GetMany()
                .Any(u => u.Id == post.UserId);

            if (!userExists)
            {
                return BadRequest("User does not exist.");
            }

            await postRepo.UpdateAsync(post);

            return NoContent();
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }

    // DELETE POST
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await postRepo.DeleteAsync(id);

            return NoContent();
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }
}