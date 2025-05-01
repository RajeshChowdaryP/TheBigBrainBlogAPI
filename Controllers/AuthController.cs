using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TheBigBrainBlog.API.Models.DTO;
using TheBigBrainBlog.API.Repositories.Interfaces;

namespace TheBigBrainBlog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITokenRepository _tokenRepository;

        public AuthController(UserManager<IdentityUser> userManager, ITokenRepository tokenRepository) {
            _userManager = userManager;
            _tokenRepository = tokenRepository;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO request)
        {
            // Create identityUser object
            var identityUser = new IdentityUser
            {
                UserName = request.Email?.Trim(),
                Email = request.Email?.Trim()
            };

            // Create the user in the database
            var identityResult = await _userManager.CreateAsync(identityUser, request.Password);

            // Check if the email already exists
            if (await CheckExistingEmail(request.Email))
            {
                ModelState.AddModelError("Message", "Email already exists");
                return ValidationProblem(ModelState);
            }

            if (identityResult.Succeeded)
            {
                // Assign a role to the user (optional)
                identityResult = await _userManager.AddToRoleAsync(identityUser, "Reader");

                // You can also create a role if it doesn't exist
                // var roleResult = await _roleManager.CreateAsync(new IdentityRole("Reader"));

                // You can also assign multiple roles if needed
                // await _userManager.AddToRolesAsync(identityUser, new[] { "Reader", "Writer" });

                // Generate a token (optional)
                //var token = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);

                // Send the token to the user via email (optional)
                // You can use an email service to send the token to the user's email address
                // For simplicity, we are just returning the token in the response
                // return Ok(new { Token = token });

                // User created successfully
                if (identityResult.Succeeded)
                {
                    return Ok(new { Message = "User registered successfully!" });
                }
                else
                {
                    // Handle errors
                    //var errors = string.Join(", ", identityResult.Errors.Select(e => e.Description));
                    //return BadRequest(new { Message = "User registration failed", Errors = errors });

                    if(identityResult.Errors.Any())
                    {
                        foreach (var error in identityResult.Errors)
                        {
                            ModelState.AddModelError("Message", error.Description);
                        }
                    }
                }
            }
            else
            {
                // Handle errors
                //var errors = string.Join(", ", identityResult.Errors.Select(e => e.Description));
                //return BadRequest(new { Message = "User registration failed", Errors = errors });

                if (identityResult.Errors.Any())
                {
                    foreach (var error in identityResult.Errors)
                    {
                        ModelState.AddModelError("Message", error.Description);
                    }
                }
            }

            return ValidationProblem(ModelState);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO request)
        {
            // Check Email
            var identityUser = await _userManager.FindByEmailAsync(request.Email);

            if(identityUser is not null)
            {
                // Check Password
                var checkPasswordResult = await _userManager.CheckPasswordAsync(identityUser, request.Password);

                if (checkPasswordResult)
                {
                    // Get Roles
                    var roles = await _userManager.GetRolesAsync(identityUser);
                    // Generate Token
                    var token = _tokenRepository.CreateJwtToken(identityUser, roles.ToList<string>());

                    // Create Token and Response
                    var response = new LoginResponseDTO()
                    {
                        Email = request.Email,
                        Token = token,
                        Roles = roles.ToList<string>()
                    };

                    return Ok(response);
                }
            }

            ModelState.AddModelError("", "Email or Password Incorrect");

            return ValidationProblem(ModelState);
        }

        private async Task<bool> CheckExistingEmail(string email)
        {
            if(string.IsNullOrEmpty(email))
            {
                return false;
            }
            var identityUser = await _userManager.FindByEmailAsync(email);
            if (identityUser is not null)
            {
                return true;
            }
            return false;
        }

        //[HttpPost("Login")]
        //public IActionResult Login([FromBody] LoginRequestDto request)
        //{
        //    // Validate the user credentials
        //    if (request.Username == "admin" && request.Password == "password")
        //    {
        //        // Generate a token (for simplicity, using a static token here)
        //        var token = "your_generated_token";
        //        return Ok(new { Token = token });
        //    }
        //    return Unauthorized();
        //}
        //[HttpPost("Register")]
        //public IActionResult Register([FromBody] RegisterRequestDto request)
        //{
        //    // Here you would typically save the user to the database
        //    // For simplicity, we are just returning a success message
        //    return Ok(new { Message = "User registered successfully!" });
        //}
    }
}
