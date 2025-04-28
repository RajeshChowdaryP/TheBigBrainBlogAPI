/* this is a important file in the application and this is the entry point in the application. the code inside this will execute first and then the application starts and
 also asp.net core has inbuilt dependency injection and
 another important function is this file is to configure the http request pipeline and by using the request pipeline we add middleware software that is assembled into 
 the application pipeline to handle responses and requests */

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
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

// inject IdentityDbContext class file to use AuthDbContext to communicate with DB in the application
builder.Services.AddDbContext<AuthDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("TheBigBrainBlogConnectionString"));
});

// this is the DI. we are telling when we inject ICategoryRepository use the implementation of ICategoryRepository in CategoryRepository
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IBlogPostRepository, BlogPostRepository>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();

// this is used to configure the identity in the application
builder.Services.AddIdentityCore<IdentityUser>(options =>
{
    //options.User.RequireUniqueEmail = true; // this is used to make the email unique in the application
}).AddRoles<IdentityRole>() // this is used to add roles in the application
.AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("TheBigBrainBlog") // this is used to add token provider in the application
.AddEntityFrameworkStores<AuthDbContext>() // this is used to add the DbContext in the application
.AddDefaultTokenProviders(); // this is used to add default token providers in the application

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings
    options.Password.RequireDigit = false; // Require at least one digit in the password
    options.Password.RequiredLength = 8; // Minimum length of the password
    options.Password.RequireNonAlphanumeric = false; // Do not require non-alphanumeric characters
    options.Password.RequireUppercase = false; // Require at least one uppercase letter in the password
    options.Password.RequireLowercase = false; // Require at least one lowercase letter in the password
    options.Password.RequiredUniqueChars = 1; // Require at least one unique character in the password

    // User settings
    //options.User.RequireUniqueEmail = true; // Require unique email addresses for users
});

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//});

// Configure JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            AuthenticationType = "Jwt",
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            //IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"])),
        };
    });

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

// this is used to use authentication in the application
app.UseAuthentication();

// this is used to use authorization in the application
app.UseAuthorization();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Images")),
    RequestPath = "/Images"
}); // this is used to serve static files like images, css, js, etc. from the wwwroot folder

app.MapControllers();

app.Run();
