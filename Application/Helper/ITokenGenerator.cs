using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Helper
{
    public interface ITokenGenerator
    {
        string Generate(int userId, string name, string role);
    }
}
