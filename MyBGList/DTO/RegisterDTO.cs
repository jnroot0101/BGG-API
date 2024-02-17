using System.ComponentModel.DataAnnotations;

namespace MyBGList.DTO;

public class RegisterDTO
{
    [Required] public string? Username { get; set; }

    [Required] [EmailAddress] public string? Email { get; set; }

    [Required] public string? Password { get; set; }
}