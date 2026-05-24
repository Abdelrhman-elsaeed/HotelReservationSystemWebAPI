using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOS.Auth
{
    public record RegisterResponseDto(int Id,string Name, string Username, string Email, Role Role);
    
}
