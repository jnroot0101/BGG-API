using System.ComponentModel.DataAnnotations;
using MyBGList.Attributes;

namespace MyBGList.DTO;

public class LoginDTO
{
    [Required]
    [MaxLength(255)]
    [CustomKeyValue("x-test-1", "value 1")]
    [CustomKeyValue("x-test-2", "value 2")]
    public string? Username { get; set; }

    [Required] public string? Password { get; set; }
}