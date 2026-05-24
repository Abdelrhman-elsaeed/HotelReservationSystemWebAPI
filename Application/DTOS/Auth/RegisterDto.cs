using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOS.Auth
{
    public record RegisterDto(string Name, string Username, string Email, string Password);
}
