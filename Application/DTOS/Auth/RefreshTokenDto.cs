using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOS.Auth
{
    public record RefreshTokenDto(string Token, string RefreshToken);
}
