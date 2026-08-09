using MudBlazor.Services;
using TukangSayurOnline.Shared.Services;
using TukangSayurOnline.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add MudBlazor Services 7.4.0
builder.Services.AddMudServices();

// Register App State, GPS Service, and API Service
builder.Services.AddSingleton<AppStateService>();
builder.Services.AddScoped<IGpsService, TukangSayurOnline.Web.Services.WebGpsService>();
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5000/";
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(apiBaseUrl.EndsWith("/") ? apiBaseUrl : apiBaseUrl + "/")
});
builder.Services.AddScoped<ApiService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(TukangSayurOnline.Shared.Routes).Assembly);

app.Run();
