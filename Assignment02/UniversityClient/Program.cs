using UniversityClient.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Адрес API берётся из конфигурации, в коде он не зашит.
var apiBaseUrl = builder.Configuration["UniversityApi:BaseUrl"]
    ?? throw new InvalidOperationException("UniversityApi:BaseUrl is not configured");

builder.Services.AddHttpClient<CoursesApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(10);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Courses}/{action=Index}/{id?}");

app.Run();
