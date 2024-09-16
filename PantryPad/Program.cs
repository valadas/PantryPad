using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo{
            Version = "1.0",
            Title = "PantryPad API v1",
            Description = "An API for PantryPad (v1)",
        });
    });
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowStencilDevServer", policy =>
    {
        policy.WithOrigins("http://localhost:3333")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowStencilDevServer");
    app.UseDeveloperExceptionPage();
}

app.UseDefaultFiles(new DefaultFilesOptions
    {
        FileProvider = new PhysicalFileProvider(
            Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "www")),
        DefaultFileNames = new List<string> { "index.html" },
    });

    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider
        (Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "www")),
        RequestPath = "",
    });
app.UseSwagger();
app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PantryPad API v1");
        c.RoutePrefix = "swagger";
    });

app.MapControllers();

app.Run("http://0.0.0.0:8099");
