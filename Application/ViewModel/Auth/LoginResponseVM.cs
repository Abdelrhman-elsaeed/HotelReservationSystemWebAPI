using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ViewModel.Auth
{

    public record LoginResponseVM(string Token, string RefreshToken, string Name, string Role);

}
