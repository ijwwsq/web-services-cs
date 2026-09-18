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

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int Year { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private static readonly List<Book> books = new()
    {
        new Book { Id = 1, Title = "Clean Code", Author = "Robert Martin", Year = 2008 },
        new Book { Id = 2, Title = "The Pragmatic Programmer", Author = "Andrew Hunt", Year = 1999 }
    };

    [HttpGet]
    public ActionResult<List<Book>> GetAll()
    {
        return Ok(books);
    }

    [HttpPost]
    public ActionResult<Book> Create(Book book)
    {
        book.Id = books.Count == 0 ? 1 : books.Max(x => x.Id) + 1;
        books.Add(book);
        return Ok(book);
    }
}
