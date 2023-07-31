﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelDiary.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string NickName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set;}
        public string PasswordHash { get; set; }
    }
}
