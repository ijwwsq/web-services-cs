using CoursesApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace CoursesApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private static readonly List<Course> courses = new()
    {
        new Course { Id = 1, Name = "Web Services", Teacher = "Teacher 1", Credits = 5 },
        new Course { Id = 2, Name = "Databases", Teacher = "Teacher 2", Credits = 4 }
    };

    [HttpGet]
    public ActionResult<List<Course>> GetAll()
    {
        return Ok(courses);
    }

    [HttpGet("{id}")]
    public ActionResult<Course> GetById(int id)
    {
        var course = courses.FirstOrDefault(x => x.Id == id);
        if (course == null)
        {
            return NotFound();
        }
        return Ok(course);
    }

    [HttpPost]
    public ActionResult<Course> Create(Course course)
    {
        course.Id = courses.Count == 0 ? 1 : courses.Max(x => x.Id) + 1;
        courses.Add(course);
        return CreatedAtAction(nameof(GetById), new { id = course.Id }, course);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, Course updated)
    {
        var course = courses.FirstOrDefault(x => x.Id == id);
        if (course == null)
        {
            return NotFound();
        }
        course.Name = updated.Name;
        course.Teacher = updated.Teacher;
        course.Credits = updated.Credits;
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var course = courses.FirstOrDefault(x => x.Id == id);
        if (course == null)
        {
            return NotFound();
        }
        courses.Remove(course);
        return NoContent();
    }
}
