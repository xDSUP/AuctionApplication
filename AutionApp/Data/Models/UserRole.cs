using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace AutionApp
{
    public partial class UserRole : IdentityRole
    {
        public UserRole() : base() { }
        public UserRole(string name) : base(name) { }
    }
}
