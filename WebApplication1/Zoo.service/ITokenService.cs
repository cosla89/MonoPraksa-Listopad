using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zoo.model;

namespace Zoo.service
{
    public interface ITokenService
    {
        LoginResponse CreateToken(User user);
    }
}
