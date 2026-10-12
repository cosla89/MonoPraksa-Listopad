using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Npgsql;
using Zoo.model;

namespace Zoo.repository
{
    public class UserRepository : IUserRepository
    {
        private readonly NpgsqlDataSource _dataSource;

        public UserRepository(NpgsqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public User? GetByUsername(string username)
        {
            using var command = _dataSource.CreateCommand(
                @"SELECT id, username, password_hash, role
                  FROM users
                  WHERE username = @username");

            command.Parameters.AddWithValue("username", username);

            using var reader = command.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            return new User
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                PasswordHash = reader.GetString(2),
                Role = reader.GetString(3)
            };

        }
        public bool Add(User user)
        {
            using var command = _dataSource.CreateCommand(
                @"INSERT INTO users (username, password_hash, role)
          VALUES (@username, @passwordHash, @role)
            ON CONFLICT (username) DO NOTHING RETURNING id");

            command.Parameters.AddWithValue("username", user.Username);
            command.Parameters.AddWithValue("passwordHash", user.PasswordHash);
            command.Parameters.AddWithValue("role", user.Role);

            var newId = command.ExecuteScalar();

            if (newId == null)
            {
                return false;
            }
            user.Id = (int)newId;
            return true;
        }
    }
}