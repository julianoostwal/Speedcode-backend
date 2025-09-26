using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using speedCode.Data;
using speedCode.Model;
using System.Security.Claims;
namespace speedCode.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ApiProfilesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ApiProfilesController(AppDbContext context)
    {
        _context = context;
    }
    // aan werken
    /*[HttpGet]
    public async Task<ActionResult<IEnumerable<Profiles>>> GetApiProfiles()
    {
        return await _context.Profiles
            .Select(x => Profiles(x))
            .ToListAsync();
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<Profiles>> GetProfile(Guid id)
    {
        var profile = await _context.Profiles.FindAsync(id);

        if (profile == null)
        {
            return NotFound();
        }

        return ProfileToDTO(profile);
    }*/

    /*[HttpPut("{id}")]
    public async Task<IActionResult> PutProfile(Guid id, Profiles profiles)
    {
        if (id != profiles.id)
        {
            return BadRequest();
        }

        var existingProfile = await _context.Profiles.FindAsync(id);
        if (existingProfile == null)
        {
            return NotFound();
        }

        existingProfile.Name = profiles.Name;
        existingProfile.Email = profile.Email;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) when (!ProfileExists(id))
        {
            return NotFound();
        }

        return NoContent();
    }

    /*[HttpPost]
    public async Task<ActionResult<TodoItemDTO>> PostTodoItem(TodoItemDTO todoDTO)
    {
        var todoItem = new TodoItem
        {
            IsComplete = todoDTO.IsComplete,
            Name = todoDTO.Name
        };

        _context.TodoItems.Add(todoItem);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetTodoItem),
            new { id = todoItem.Id },
            ItemToDTO(todoItem));
    }
    
    /*[HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodoItem(long id)
    {
        var todoItem = await _context.TodoItems.FindAsync(id);
        if (todoItem == null)
        {
            return NotFound();
        }

        _context.TodoItems.Remove(todoItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool TodoItemExists(long id)
    {
        return _context.TodoItems.Any(e => e.Id == id);
    }

    private static TodoItemDTO ItemToDTO(TodoItem todoItem) =>
       new TodoItemDTO
       {
           Id = todoItem.Id,
           Name = todoItem.Name,
           IsComplete = todoItem.IsComplete
       };
    */
}