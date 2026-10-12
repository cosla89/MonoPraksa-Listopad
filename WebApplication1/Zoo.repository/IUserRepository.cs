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
        User? GetByUsername (string  username);
        bool Add(User user);

    }
}
