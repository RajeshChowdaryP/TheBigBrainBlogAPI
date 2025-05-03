using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
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

        [HttpGet("GetUsersWithRoles")]
        public async Task<IActionResult> GetUsersWithRoles()
        {
            var users = _userManager.Users.ToList();
            var usersWithRoles = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                usersWithRoles.Add(new
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Roles = roles
                });
            }

            return Ok(usersWithRoles);
        }

        [HttpPut("UpdateUserRoleByEmailId")]
        public async Task<IActionResult> UpdateUserRoleByEmailId([FromBody] UpdateUserRoleDTO request)
        {
            // Validate input  
            if (string.IsNullOrEmpty(request.Email) || request.Roles == null || !request.Roles.Any())
            {
                ModelState.AddModelError("Message", "Email and at least one role are required.");
                return ValidationProblem(ModelState);
            }

            // Find user by email  
            var identityUser = await _userManager.FindByEmailAsync(request.Email);
            if (identityUser == null)
            {
                ModelState.AddModelError("Message", "User not found.");
                return ValidationProblem(ModelState);
            }

            // Remove existing roles  
            var currentRoles = await _userManager.GetRolesAsync(identityUser);
            var removeRolesResult = await _userManager.RemoveFromRolesAsync(identityUser, currentRoles);
            if (!removeRolesResult.Succeeded)
            {
                foreach (var error in removeRolesResult.Errors)
                {
                    ModelState.AddModelError("Message", error.Description);
                }
                return ValidationProblem(ModelState);
            }

            // Add new roles  
            foreach (var role in request.Roles)
            {
                var addRoleResult = await _userManager.AddToRoleAsync(identityUser, role);
                if (!addRoleResult.Succeeded)
                {
                    foreach (var error in addRoleResult.Errors)
                    {
                        ModelState.AddModelError("Message", error.Description);
                    }
                    return ValidationProblem(ModelState);
                }
            }

            return Ok(new { Message = "User role updated successfully." });
        }

        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUser([FromBody] DeleteUserDTO user)
        {
            // Validate input  
            if (string.IsNullOrEmpty(user.email))
            {
                ModelState.AddModelError("Message", "Email is required.");
                return ValidationProblem(ModelState);
            }

            // Find user by email  
            var identityUser = await _userManager.FindByEmailAsync(user.email);
            if (identityUser == null)
            {
                ModelState.AddModelError("Message", "User not found.");
                return ValidationProblem(ModelState);
            }

            // Delete user  
            var result = await _userManager.DeleteAsync(identityUser);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Message", error.Description);
                }
                return ValidationProblem(ModelState);
            }

            return Ok(new { Message = "User deleted successfully." });
        }

        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO request)
        {
            // Validate input
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.CurrentPassword) || string.IsNullOrEmpty(request.NewPassword))
            {
                ModelState.AddModelError("Message", "Email, current password, and new password are required.");
                return ValidationProblem(ModelState);
            }

            // Find user by email
            var identityUser = await _userManager.FindByEmailAsync(request.Email);
            if (identityUser == null)
            {
                ModelState.AddModelError("Message", "User not found.");
                return ValidationProblem(ModelState);
            }

            // Change password
            var result = await _userManager.ChangePasswordAsync(identityUser, request.CurrentPassword, request.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Message", error.Description);
                }
                return ValidationProblem(ModelState);
            }

            return Ok(new { Message = "Password changed successfully." });
        }




        //public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO request)
        //{
        //    // Check if the email exists
        //    var identityUser = await _userManager.FindByEmailAsync(request.Email);
        //    if (identityUser == null)
        //    {
        //        return BadRequest(new { Message = "Email not found" });
        //    }
        //    // Generate a password reset token
        //    var token = await _userManager.GeneratePasswordResetTokenAsync(identityUser);
        //    var resetLink = Url.Action("ResetPassword", "Auth", new { token, email = identityUser.Email }, Request.Scheme);
        //    // use SMTP to send the reset link to the user's email
            



        //}

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

        //private void SendEmail(string email, string resetLink)
        //{
        //    // SMTP configuration  
        //    var fromAddress = new MailAddress("noreply.mockemail@example.com", "TheBigBrainBlog Support");
        //    var toAddress = new MailAddress(email);
        //    const string fromPassword = "MockEmailPassword123"; // Replace with a secure password  
        //    const string subject = "Password Reset Request";
        //    string body = $"Please use the following link to reset your password: {resetLink}";

        //    var smtp = new SmtpClient
        //    {
        //        Host = "smtp.example.com", // Replace with your SMTP server  
        //        Port = 587,
        //        EnableSsl = true,
        //        DeliveryMethod = SmtpDeliveryMethod.Network,
        //        UseDefaultCredentials = false,
        //        Credentials = new System.Net.NetworkCredential(fromAddress.Address, fromPassword)
        //    };

        //    using (var message = new MailMessage(fromAddress, toAddress)
        //    {
        //        Subject = subject,
        //        Body = body
        //    })
        //    {
        //        smtp.Send(message);
        //    }
        //}

    }
}
