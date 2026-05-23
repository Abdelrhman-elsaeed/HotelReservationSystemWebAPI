using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOS.User
{
    public class AddUserDto
    {
        public int? ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Role Role { get; set; } = Role.Customer;
    }
}
