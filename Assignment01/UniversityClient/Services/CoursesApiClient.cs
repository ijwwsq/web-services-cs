using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using UniversityClient.Models;

namespace UniversityClient.Services;

public class CoursesApiClient
{
    private readonly HttpClient http;
    private readonly ILogger<CoursesApiClient> logger;

    public CoursesApiClient(HttpClient http, ILogger<CoursesApiClient> logger)
    {
        this.http = http;
        this.logger = logger;
    }

    public Task<ApiCallResult<List<CourseItem>>> GetCoursesAsync() =>
        SendAsync<List<CourseItem>>("api/v1/courses");

    public Task<ApiCallResult<CourseItem>> GetCourseAsync(int id) =>
        SendAsync<CourseItem>("api/v1/courses/" + id);

    private async Task<ApiCallResult<T>> SendAsync<T>(string path)
    {
        try
        {
            using var response = await http.GetAsync(path);
            var body = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();

            var payload = body == null ? default : body.Result;

            if (response.IsSuccessStatusCode && payload is not null)
            {
                return ApiCallResult<T>.Ok(payload);
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return ApiCallResult<T>.Fail("Запрошенные данные не найдены");
            }

            var message = body?.ErrorMessage ?? response.ReasonPhrase ?? "неизвестная ошибка";
            logger.LogWarning("API ответил кодом {StatusCode}: {Message}", (int)response.StatusCode, message);
            return ApiCallResult<T>.Fail($"API вернул {(int)response.StatusCode}: {message}");
        }
        catch (HttpRequestException exception)
        {
            logger.LogError(exception, "Не удалось соединиться с API по адресу {BaseAddress}", http.BaseAddress);
            return ApiCallResult<T>.Fail("API недоступен, проверьте, что сервис запущен");
        }
        catch (TaskCanceledException)
        {
            logger.LogError("Запрос к API превысил таймаут");
            return ApiCallResult<T>.Fail("API не ответил вовремя");
        }
        catch (JsonException exception)
        {
            logger.LogError(exception, "API вернул неожиданный формат ответа");
            return ApiCallResult<T>.Fail("API вернул ответ в неожиданном формате");
        }
    }
}
