using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Zoo.model;
using Zoo.repository;

namespace Zoo.service
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ITokenService _tokenService;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher<User> passwordHasher,
            ITokenService tokenService)
         {
             _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        public string? Register(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username))
            {
                return "Username needs to be added.";
            }

            var username = request.Username.Trim().ToLowerInvariant();

            if (username.Length > 100)
            {
                return "Username max length is 100.";
            }

            if (string.IsNullOrWhiteSpace(request.Password)
                || request.Password.Length < 10
                || request.Password.Length > 128)
            {
                return "Password needs to be between 10 and 128 char.";
            }

            var existingUser = _userRepository.GetByUsername(username);

            if (existingUser != null)
            {
                return "Username taken.";
            }

            var user = new User
            {
                Username = username,
                Role = "User"
            };

            user.PasswordHash =
                _passwordHasher.HashPassword(user, request.Password);

            var added = _userRepository.Add(user);
            if (!added)
            {
                return "Username taken";
            }
            return null;
        }
        public LoginResponse? Login(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username)
                || string.IsNullOrWhiteSpace(request.Password))
            {
                return null;
            }

            var username = request.Username.Trim().ToLowerInvariant();

            if (username.Length > 100 || request.Password.Length > 128)
            {
                return null;
            }

            var user = _userRepository.GetByUsername(username);

            if (user == null)
            {
                return null;
            }

            var verification = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password);

            if (verification == PasswordVerificationResult.Failed)
            {
                return null;
            }

            return _tokenService.CreateToken(user);
        }
    }
}
