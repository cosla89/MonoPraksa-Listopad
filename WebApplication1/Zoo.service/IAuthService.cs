using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zoo.model;

namespace Zoo.service
{
    public interface IAuthService
    {
        string? Register(RegisterRequest request);
        LoginResponse? Login(LoginRequest request);
    }
}
