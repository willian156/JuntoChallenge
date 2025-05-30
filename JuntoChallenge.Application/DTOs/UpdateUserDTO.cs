﻿using JuntoChallenge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JuntoChallenge.Application.DTOs
{
    public class UpdateUserDTO
    {
        
        public string Username { get; set; }
        public string Email { get; set; }

        public UpdateUserDTO() { }
        public UpdateUserDTO(User user)
        {
            Username = user.Username;
            Email = user.Email;
        }
    }
}
