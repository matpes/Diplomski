using System;
using System.Collections.Generic;
using API.Extensions;

namespace API.Entitites
{
    public class AppUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateTime dateOfBirth { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string mail { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public char gender { get; set; }

        public ICollection<Cart> carts { get; set; }
        
    }
}