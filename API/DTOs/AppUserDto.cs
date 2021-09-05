using System;

namespace API.DTOs
{
    public class AppUserDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public int age { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string mail { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public char gender { get; set; }
    }
}