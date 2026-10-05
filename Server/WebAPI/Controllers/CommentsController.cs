using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentRepository commentRepo;
    private readonly IUserRepository userRepo;
    private readonly IPostRepository postRepo;

    public CommentsController(
        ICommentRepository commentRepo,
        IUserRepository userRepo,
        IPostRepository postRepo)
    {
        this.commentRepo = commentRepo;
        this.userRepo = userRepo;
        this.postRepo = postRepo;
    }

    // CREATE COMMENT
    [HttpPost]
    public async Task<ActionResult<Comment>> AddComment(
        [FromBody] Comment comment)
    {
        bool userExists = userRepo
            .GetMany()
            .Any(u => u.Id == comment.UserId);

        if (!userExists)
        {
            return BadRequest("User does not exist.");
        }

        bool postExists = postRepo
            .GetMany()
            .Any(p => p.Id == comment.PostId);

        if (!postExists)
        {
            return BadRequest("Post does not exist.");
        }

        Comment created =
            await commentRepo.AddAsync(comment);

        return Created(
            $"/Comments/{created.Id}",
            created);
    }

    // GET ALL COMMENTS + FILTER
    [HttpGet]
    public ActionResult<IEnumerable<Comment>> GetMany(
        [FromQuery] int? userId,
        [FromQuery] string? userName,
        [FromQuery] int? postId)
    {
        IQueryable<Comment> comments =
            commentRepo.GetMany();

        // Filter by user ID
        if (userId.HasValue)
        {
            comments = comments.Where(c =>
                c.UserId == userId.Value);
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
                return Ok(new List<Comment>());
            }

            comments = comments.Where(c =>
                c.UserId == user.Id);
        }

        // Filter by post ID
        if (postId.HasValue)
        {
            comments = comments.Where(c =>
                c.PostId == postId.Value);
        }

        return Ok(comments.ToList());
    }

    // GET ONE COMMENT
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Comment>> GetSingle(int id)
    {
        try
        {
            Comment comment =
                await commentRepo.GetSingleAsync(id);

            return Ok(comment);
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }

    // UPDATE COMMENT
    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(
        int id,
        [FromBody] Comment comment)
    {
        try
        {
            comment.Id = id;

            bool userExists = userRepo
                .GetMany()
                .Any(u => u.Id == comment.UserId);

            if (!userExists)
            {
                return BadRequest("User does not exist.");
            }

            bool postExists = postRepo
                .GetMany()
                .Any(p => p.Id == comment.PostId);

            if (!postExists)
            {
                return BadRequest("Post does not exist.");
            }

            await commentRepo.UpdateAsync(comment);

            return NoContent();
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }

    // DELETE COMMENT
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await commentRepo.DeleteAsync(id);

            return NoContent();
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }
}