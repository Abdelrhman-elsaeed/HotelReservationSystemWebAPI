using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOS.User
{
    public record LoginResponseDto(string Token, string Name, string Role);
}
