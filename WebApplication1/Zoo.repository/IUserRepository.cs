using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zoo.model;

namespace Zoo.repository
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync (string  username);
        Task<bool> AddAsync(User user);

    }
}
