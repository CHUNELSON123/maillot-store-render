using Microsoft.AspNetCore.Http;

namespace MaillotStore.Services.Implementations
{
    public class ReferralService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string ReferralCookieKey = "MaillotStoreReferralCode";

        public ReferralService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // Call this when the app loads to check the URL
        public void SetReferralCodeFromQuery(string? refCode)
        {
            if (!string.IsNullOrEmpty(refCode))
            {
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(30), // Cookie lasts for 30 days
                    HttpOnly = true,
                    Secure = true,
                    IsEssential = true
                };
                _httpContextAccessor.HttpContext?.Response.Cookies.Append(ReferralCookieKey, refCode, cookieOptions);
            }
        }

        // Call this during checkout to get the stored code
        public string? GetReferralCode()
        {
            return _httpContextAccessor.HttpContext?.Request.Cookies[ReferralCookieKey];
        }

        // --- START: Added Missing Method ---
        /// <summary>
        /// Checks if a referral cookie exists for the current user.
        /// </summary>
        /// <returns>True if the referral cookie is present, otherwise false.</returns>
        public bool IsReferred()
        {
            // Return true if the browser's request contains a cookie with our specific key
            return _httpContextAccessor.HttpContext?.Request.Cookies.ContainsKey(ReferralCookieKey) ?? false;
        }
        // --- END: Added Missing Method ---
    }
}