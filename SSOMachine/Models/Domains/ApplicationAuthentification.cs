using SSOMachine.Models.Domains;

namespace SSOService.Models.Domains
{
    public class ApplicationAuthentification : EntityTracking
    {
        public string Token { get; set; }
        public long ClientApplicationId { get; set; }
        public long ServerApplicationId { get; set; }
    }
}
