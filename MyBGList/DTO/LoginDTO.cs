using System.ComponentModel.DataAnnotations;

namespace MyBGList.DTO;

public class LoginDTO
{
    [Required] [MaxLength(255)] public string? Username { get; set; }

    [Required] public string? Password { get; set; }
}