using Microsoft.AspNetCore.Identity;

namespace MaillotStore.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }

        public string? ReferralCode { get; set; }
        public DateTime JoinedDate { get; set; }
    }

}
