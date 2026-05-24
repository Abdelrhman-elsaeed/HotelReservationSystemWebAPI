using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ViewModel.Auth
{
    public record RegisterVM(string Name, string Username, string Email, string Password);
}
