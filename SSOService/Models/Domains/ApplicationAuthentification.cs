using SSOMachine.Models.Domains;
using System;

namespace SSOService.Models.Domains
{
    public class ApplicationAuthentification : EntityTracking
    {
        public string Token { get; set; }
        public Guid ClientApplicationId { get; set; }
        public Guid ServerApplicationId { get; set; }
    }
}
