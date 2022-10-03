using System;

namespace SSOService.Models.Domains
{
    // This is the authentication between communicating applications
    public class ApplicationAuthentication : EntityTracking
    {
        public ApplicationAuthentication()
        {
            ClientSecret =  Guid.NewGuid().ToString();
        }
        public long ApplicationAuthenticationId { get; set; }
        public string ClientSecret { get; set; }
        public long ClientApplicationId { get; set; }
        public Application ClientApplication { get; set; }
        public long ServerApplicationId { get; set; }
        public Application ServerApplication { get; set; }


    }
}
