using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Dtos
{
    public class USerForRegisterDto
    {
        [Required]
        public string Username { get; set; }

         [Required]
         [StringLength(8, MinimumLength = 4, ErrorMessage = "You Must Specified password between 4-8")]
        public string Password { get; set; }
    }
    
}