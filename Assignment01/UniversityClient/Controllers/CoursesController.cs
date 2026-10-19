using Microsoft.AspNetCore.Mvc;
using UniversityClient.Services;

namespace UniversityClient.Controllers;

public class CoursesController : Controller
{
    private readonly CoursesApiClient api;

    public CoursesController(CoursesApiClient api)
    {
        this.api = api;
    }

    public async Task<IActionResult> Index()
    {
        var result = await api.GetCoursesAsync();

        if (!result.Success)
        {
            ViewBag.Error = result.ErrorMessage;
            return View(new List<Models.CourseItem>());
        }

        return View(result.Data);
    }

    public async Task<IActionResult> Details(int id)
    {
        var result = await api.GetCourseAsync(id);

        if (!result.Success)
        {
            ViewBag.Error = result.ErrorMessage;
            return View(null as Models.CourseItem);
        }

        return View(result.Data);
    }
}
