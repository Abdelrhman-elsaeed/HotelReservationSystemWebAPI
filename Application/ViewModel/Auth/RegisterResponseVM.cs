using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ViewModel.Auth
{
    public record RegisterResponseVM(int Id, string Name, string Username, string Email, Role Role);
}
