using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.ApplicationLayer.Dtos.Users
{
    public class ChangeRoleDto
    {
        public string UserId { get; set; }
        public string NewRole { get; set; }
    }
}
