using System.ComponentModel.DataAnnotations;

namespace OpenHIoT.LocalServer.Models
{
    public class LoginDTO
    {
        [Required]
        public string Policy { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
