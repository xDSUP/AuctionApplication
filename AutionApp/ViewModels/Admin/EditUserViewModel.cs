using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutionApp.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public IList<string> UserRoles { get; set; }
        public IList<UserRole> AllRoles { get; set; }
    }
}
