using System;

namespace SSOService.Models.Domains
{
    // This is the authentication between communicating applications
    public class ApplicationAuthentication : EntityTracking
    {
        public ApplicationAuthentication()
        {
            ClientSecret = Guid.NewGuid().ToString();
        }
        public string ClientSecret { get; set; }
        public Guid ClientApplicationId { get; set; }
        public Guid ServerApplicationId { get; set; }


    }
}
