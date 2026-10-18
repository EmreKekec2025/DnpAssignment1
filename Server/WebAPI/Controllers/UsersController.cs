using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository userRepository;

    public UsersController(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto request)
    {
        User user = new User { Username = request.Username, Password = request.Password };
        User created = await userRepository.AddAsync(user);

        UserDto dto = new UserDto { Id = created.Id, Username = created.Username };
        return Created($"/users/{dto.Id}", dto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateUser(int id, [FromBody] CreateUserDto request)
    {
        User user = new User { Id = id, Username = request.Username, Password = request.Password };
        await userRepository.UpdateAsync(user);
        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetSingleUser(int id)
    {
        User user = await userRepository.GetSingleAsync(id);
        UserDto dto = new UserDto { Id = user.Id, Username = user.Username };
        return Ok(dto);
    }

    [HttpGet]
    public ActionResult<IEnumerable<UserDto>> GetManyUsers([FromQuery] string? username)
    {
        IQueryable<User> users = userRepository.GetManyAsync();

        if (!string.IsNullOrWhiteSpace(username))
        {
            users = users.Where(u => u.Username.Contains(username));
        }

        List<UserDto> dtos = users
            .Select(u => new UserDto { Id = u.Id, Username = u.Username })
            .ToList();

        return Ok(dtos);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        await userRepository.DeleteAsync(id);
        return NoContent();
    }
}