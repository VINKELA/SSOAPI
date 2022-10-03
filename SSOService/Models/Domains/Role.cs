using System;

namespace SSOService.Models.Domains
{
    //Roles created by clients for their users
    public class Role : EntityTracking
    {
        public long RoleId { get; set; }
        public string Name { get; set; }
        public long ClientId { get; set; }
        public Client Client { get; set; }
    }
}
