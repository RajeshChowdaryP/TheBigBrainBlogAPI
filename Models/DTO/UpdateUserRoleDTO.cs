namespace TheBigBrainBlog.API.Models.DTO
{
    public class UpdateUserRoleDTO
    {
        public string Email { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
    }
}
