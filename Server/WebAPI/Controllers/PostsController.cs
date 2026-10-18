using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostRepository postRepository;

    public PostsController(IPostRepository postRepository)
    {
        this.postRepository = postRepository;
    }

    [HttpPost]
    public async Task<ActionResult<PostDto>> CreatePost([FromBody] CreatePostDto request)
    {
        Post post = new Post { Title = request.Title, Body = request.Body, UserId = request.UserId };
        Post created = await postRepository.AddAsync(post);

        PostDto dto = new PostDto { Id = created.Id, Title = created.Title, Body = created.Body, UserId = created.UserId };
        return Created($"/posts/{dto.Id}", dto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdatePost(int id, [FromBody] CreatePostDto request)
    {
        Post post = new Post { Id = id, Title = request.Title, Body = request.Body, UserId = request.UserId };
        await postRepository.UpdateAsync(post);
        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PostDto>> GetSinglePost(int id)
    {
        Post post = await postRepository.GetSingleAsync(id);
        PostDto dto = new PostDto { Id = post.Id, Title = post.Title, Body = post.Body, UserId = post.UserId };
        return Ok(dto);
    }

    [HttpGet]
    public ActionResult<IEnumerable<PostDto>> GetManyPosts([FromQuery] string? title, [FromQuery] int? userId)
    {
        IQueryable<Post> posts = postRepository.GetManyAsync();

        if (!string.IsNullOrWhiteSpace(title))
        {
            posts = posts.Where(p => p.Title.Contains(title));
        }

        if (userId is not null)
        {
            posts = posts.Where(p => p.UserId == userId);
        }

        List<PostDto> dtos = posts
            .Select(p => new PostDto { Id = p.Id, Title = p.Title, Body = p.Body, UserId = p.UserId })
            .ToList();

        return Ok(dtos);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletePost(int id)
    {
        await postRepository.DeleteAsync(id);
        return NoContent();
    }
}