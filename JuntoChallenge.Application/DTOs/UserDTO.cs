using JuntoChallenge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JuntoChallenge.Application.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool isDeleted { get; set; }

        public UserDTO() { }

        public UserDTO(User user)
        {
            Id = user.Id;
            Username = user.Username;
            Email = user.Email;
            isDeleted = user.IsDeleted;
        }
    }
}
