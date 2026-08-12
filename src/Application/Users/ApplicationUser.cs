using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public Guid TenantId { get; set; }
    }

}
