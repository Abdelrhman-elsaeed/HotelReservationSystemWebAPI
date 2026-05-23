using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOS.User
{
    public class AddUserResponseDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public Role Role { get; set; }
    }
}
