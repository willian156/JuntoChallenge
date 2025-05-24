using JuntoChallenge.Application.DTOs;
using JuntoChallenge.Application.Interfaces;
using JuntoChallenge.Domain.Entities;
using JuntoChallenge.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JuntoChallenge.Application.Services
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        public UserService(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        

        public List<UserDTO> GetUsers(int pageNumber, int pageSize)
        {
            var query = _context.Users.AsQueryable();

            var pagedList = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(usr => new UserDTO(usr))
                .ToList();

            return pagedList;
        }

        public UserDTO GetUser(int id)
        {
            var usr = _context.Users.Select(usr => new UserDTO(usr)).FirstOrDefault();
            return usr;
        }

        public async Task<UserDTO> UpdateUser(int id, UpdateUserDTO userDTO)
        {
            try
            {
                var oldUser = await _context.Users.FindAsync(id);

                if (oldUser == null) 
                {
                    return null;
                }

                if (userDTO.Username != "" || userDTO.Email != "")
                {
                    if (userDTO.Username != "")
                    {
                        oldUser.Username = userDTO.Username;
                    }
                    if (userDTO.Email != "")
                    {
                        oldUser.Email = userDTO.Email;
                    }
                }
                else
                {
                    return null;
                }

                _context.Entry(oldUser).State = EntityState.Modified;
                _context.Users.Update(oldUser);
                await _context.SaveChangesAsync();

                var dtoUser = new UserDTO(oldUser);

                return dtoUser;
            }
            catch (DbUpdateConcurrencyException) 
            {
                if (!UserExists(id))
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<UserDTO?> PostUser(PostUserDTO user)
        {
            try
            {
                if(user.Username == "")
                {
                    throw new ArgumentException("Username can't be empty!", nameof(user.Username));
                }
                if (user.Email == "")
                {
                    throw new ArgumentException("Email can't be empty!", nameof(user.Email));
                }
                if (user.Password == "")
                {
                    throw new ArgumentException("Password can't be empty!", nameof(user.Password));
                }

                var sameUser = _context.Users.Where(usr => usr.Username == user.Username || usr.Email == user.Email).FirstOrDefault();
                
                if (sameUser != null)
                {
                    if(sameUser.Username == user.Username)
                    {
                        throw new ArgumentException("Username already registered!", nameof(user.Username));
                    }
                    if(sameUser.Email == user.Email)
                    {
                        throw new ArgumentException("Email already registered!", nameof(user.Email));
                    }
                }


                var hashPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);

                var newUser = new User()
                {
                    Username = user.Username,
                    Email = user.Email,
                    Password = hashPassword,
                };

                _context.Users.Add(newUser);
                var post = await _context.SaveChangesAsync();
                if (post > 0)
                {
                    var dtoUser = new UserDTO(newUser);
                    return dtoUser;
                }else if (post == 0)
                {
                    return new UserDTO();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> DeleteUser(int id)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == id);

                if (user == null || user.IsDeleted)
                    return false;

                user.IsDeleted = true;

                _context.Entry(user).State = EntityState.Modified;
                _context.Users.Update(user);
                _context.SaveChanges();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        public LoginResponseDTO Login(LoginDTO login)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == login.Username);

            if (user == null || user.IsDeleted)
            {
                throw new Exception("User not found!");
            }

            if (!BCrypt.Net.BCrypt.Verify(login.Password, user.Password))
            {
                throw new Exception("Incorrect password!");
            }
            
            var token = GenerateJwt(user, _config);

            return new LoginResponseDTO
            {
                Id = user.Id,
                Username = user.Username,
                JWT = token
            };

        }

        public async Task<bool> UpdatePassword(UpdatePasswordUserDTO userDTO)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userDTO.Username) ||
                    string.IsNullOrWhiteSpace(userDTO.OldPassword) ||
                    string.IsNullOrWhiteSpace(userDTO.NewPassword))
                {
                    return false; 
                }

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == userDTO.Username);

                if (user == null)
                    return false; 

                if (!BCrypt.Net.BCrypt.Verify(userDTO.OldPassword, user.Password))
                    return false; 

                
                if (BCrypt.Net.BCrypt.Verify(userDTO.NewPassword, user.Password))
                    return false; 

                user.Password = BCrypt.Net.BCrypt.HashPassword(userDTO.NewPassword);

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        private string GenerateJwt(User user, IConfiguration config)
        {
            var jwtSettings = config.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email)
        }),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpireMinutes"])),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
