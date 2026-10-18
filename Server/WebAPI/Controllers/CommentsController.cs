using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentRepository commentRepository;

    public CommentsController(ICommentRepository commentRepository)
    {
        this.commentRepository = commentRepository;
    }

    [HttpPost]
    public async Task<ActionResult<CommentDto>> CreateComment([FromBody] CreateCommentDto request)
    {
        Comment comment = new Comment { Body = request.Body, PostId = request.PostId, UserId = request.UserId };
        Comment created = await commentRepository.AddAsync(comment);

        CommentDto dto = new CommentDto { Id = created.Id, Body = created.Body, PostId = created.PostId, UserId = created.UserId };
        return Created($"/comments/{dto.Id}", dto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateComment(int id, [FromBody] CreateCommentDto request)
    {
        Comment comment = new Comment { Id = id, Body = request.Body, PostId = request.PostId, UserId = request.UserId };
        await commentRepository.UpdateAsync(comment);
        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CommentDto>> GetSingleComment(int id)
    {
        Comment comment = await commentRepository.GetSingleAsync(id);
        CommentDto dto = new CommentDto { Id = comment.Id, Body = comment.Body, PostId = comment.PostId, UserId = comment.UserId };
        return Ok(dto);
    }

    [HttpGet]
    public ActionResult<IEnumerable<CommentDto>> GetManyComments([FromQuery] int? userId, [FromQuery] int? postId)
    {
        IQueryable<Comment> comments = commentRepository.GetManyAsync();

        if (userId is not null)
        {
            comments = comments.Where(c => c.UserId == userId);
        }

        if (postId is not null)
        {
            comments = comments.Where(c => c.PostId == postId);
        }

        List<CommentDto> dtos = comments
            .Select(c => new CommentDto { Id = c.Id, Body = c.Body, PostId = c.PostId, UserId = c.UserId })
            .ToList();

        return Ok(dtos);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteComment(int id)
    {
        await commentRepository.DeleteAsync(id);
        return NoContent();
    }
}