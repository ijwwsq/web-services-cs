using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private static readonly List<TaskItem> tasks = new()
    {
        new TaskItem { Id = 1, Title = "Read the module", Description = "ASP.NET Core Web API", IsCompleted = true },
        new TaskItem { Id = 2, Title = "Write the controller", Description = "CRUD for tasks", IsCompleted = false },
        new TaskItem { Id = 3, Title = "Test in Swagger", Description = "GET, POST, PUT, DELETE", IsCompleted = false }
    };

    [HttpGet]
    public ActionResult<List<TaskItem>> GetAll()
    {
        return Ok(tasks);
    }

    [HttpGet("{id}")]
    public ActionResult<TaskItem> GetById(int id)
    {
        var task = tasks.FirstOrDefault(x => x.Id == id);
        if (task == null)
        {
            return NotFound();
        }
        return Ok(task);
    }

    [HttpPost]
    public ActionResult<TaskItem> Create(TaskItem task)
    {
        if (string.IsNullOrWhiteSpace(task.Title))
        {
            return BadRequest("Title is required");
        }
        task.Id = tasks.Count == 0 ? 1 : tasks.Max(x => x.Id) + 1;
        tasks.Add(task);
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
    }

    [HttpPut("{id}")]
    public ActionResult<TaskItem> Update(int id, TaskItem updated)
    {
        if (string.IsNullOrWhiteSpace(updated.Title))
        {
            return BadRequest("Title is required");
        }
        var task = tasks.FirstOrDefault(x => x.Id == id);
        if (task == null)
        {
            return NotFound();
        }
        task.Title = updated.Title;
        task.Description = updated.Description;
        task.IsCompleted = updated.IsCompleted;
        return Ok(task);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var task = tasks.FirstOrDefault(x => x.Id == id);
        if (task == null)
        {
            return NotFound();
        }
        tasks.Remove(task);
        return Ok();
    }
}
