/* this is a important file in the application and this is the entry point in the application. the code inside this will execute first and then the application starts and
 also asp.net core has inbuilt dependency injection and
 another important function is this file is to configure the http request pipeline and by using the request pipeline we add middleware software that is assembled into 
 the application pipeline to handle responses and requests */

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using TheBigBrainBlog.API.Data;
using TheBigBrainBlog.API.Repositories.Implementation;
using TheBigBrainBlog.API.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// do DI before build

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor(); // IHttpContextAccessor is used to access the current HTTP context
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    //c =>
//{
//    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

//    // Optional: if you want to allow file uploads for specific actions
//    c.OperationFilter<FileUploadOperationFilter>();
//}
);

//inject DbContext class file to use ApplicationDbContext to communicate with DB in the application
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("TheBigBrainBlogConnectionString"));
});
// this is the DI. we are telling when we inject ICategoryRepository use the implementation of ICategoryRepository in CategoryRepository
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IBlogPostRepository, BlogPostRepository>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();

// application build
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// these are middlewares that have been configured in the application the order that have been occured is also important

app.UseHttpsRedirection();

//  cors setup
app.UseCors(options =>
{
    options.AllowAnyHeader();
    options.AllowAnyOrigin();
    options.AllowAnyMethod();
});

app.UseAuthorization();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Images")),
    RequestPath = "/Images"
}); // this is used to serve static files like images, css, js, etc. from the wwwroot folder

app.MapControllers();

app.Run();
