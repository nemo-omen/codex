using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Codex.Api.Data;
using Codex.Api.Features.Bookmarks;
using Codex.Api.Features.Identity;
using Codex.Api.Features.Notes;
using Codex.Api.Models;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// default JSON serializer options
builder.Services.Configure<JsonOptions>(options =>
{
	options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
	options.SerializerOptions.WriteIndented = false;
	options.SerializerOptions.Encoder = JavaScriptEncoder.Default;
	options.SerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
	options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
	options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddIdentityApiEndpoints<ApplicationUser>(options =>
	{
		options.Password.RequireNonAlphanumeric = false;
	})
	.AddRoles<IdentityRole>()
	.AddEntityFrameworkStores<ApplicationDbContext>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var onlyAllowLocalhostOrigins = "_onlyAllowLocalhostOrigins";

builder.Services.AddCors(options =>
{
	options.AddPolicy(name: onlyAllowLocalhostOrigins,
		policy => { policy.WithOrigins("http://localhost"); });
});

string connectionString;

if (builder.Environment.IsDevelopment())
{
	connectionString = builder.Configuration.GetConnectionString("DevConnection") ??
		throw new InvalidOperationException("Connection string 'DevConnection' not found.");
}
else
{
	connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
		throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
	options.UseNpgsql(connectionString);
	options.EnableDetailedErrors();
}, ServiceLifetime.Transient);

builder.Services.AddScoped<IBookmarksRepository, BookmarksRepository>();
builder.Services.AddScoped<IBookmarksService, BookmarksService>();
builder.Services.AddScoped<INotesRepository, NotesRepository>();
builder.Services.AddScoped<INotesService, NotesService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

app.UseCors();

app.UseHttpsRedirection();

app.CustomMapIdentityApi<ApplicationUser>()
	.RequireCors(onlyAllowLocalhostOrigins);

app.UseAuthorization();

app.MapControllers();

app.Run();