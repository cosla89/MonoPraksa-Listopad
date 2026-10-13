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
       Task<string?> RegisterAsync(RegisterRequest request);
        Task<LoginResponse?> LoginAsync(LoginRequest request);
    }
}
