using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDto
    {
        [Required] public string Username { get; set; }
        [Required] public string Password { get; set; }
        [Required] public string name { get; set; }
        [Required] public string surname { get; set; }
        [Required] public string mail { get; set; }
        [Required] public string gender { get; set; }
        [Required] public DateTime dateOfBirth { get; set; }
    }
}