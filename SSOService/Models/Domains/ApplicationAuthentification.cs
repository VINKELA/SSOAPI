using SSOMachine.Models.Domains;
using System;

namespace SSOService.Models.Domains
{
    // This is the authentication between communicating applications
    public class ApplicationAuthentification : EntityTracking
    {
        public ApplicationAuthentification()
        {
            ClientSecret = Guid.NewGuid().ToString();
        }
        public string ClientSecret { get; set; }
        public Guid ClientApplicationId { get; set; }
        public Guid ServerApplicationId { get; set; }
    }
}
