using Microsoft.AspNetCore.Http;

namespace MaillotStore.Services.Implementations
{
    public class ReferralService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string ReferralCookieKey = "MaillotStoreReferralCode";

        // This variable holds the code once we capture it
        private string? _currentReferralCode;

        public ReferralService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // --- 1. SETTER: Called by Routes.razor or App.razor to "hydrate" the service ---
        public void SetReferralCode(string code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                _currentReferralCode = code;
            }
        }

        // --- 2. GETTER: Used by Checkout and other pages ---
        public string? GetReferralCode()
        {
            // If we already have the code in memory, return it
            if (!string.IsNullOrEmpty(_currentReferralCode))
            {
                return _currentReferralCode;
            }

            // Fallback: Try to read from cookies (Only works during initial load)
            var cookieCode = _httpContextAccessor.HttpContext?.Request.Cookies[ReferralCookieKey];
            if (!string.IsNullOrEmpty(cookieCode))
            {
                _currentReferralCode = cookieCode;
            }

            return _currentReferralCode;
        }

        public bool IsReferred()
        {
            return !string.IsNullOrEmpty(GetReferralCode());
        }

        // --- 3. URL HANDLER: Called when user first arrives via a link ---
        public void SetReferralCodeFromQuery(string? refCode)
        {
            if (!string.IsNullOrEmpty(refCode))
            {
                _currentReferralCode = refCode;

                // --- FIX: Determine if we are on HTTPS ---
                bool isSecure = _httpContextAccessor.HttpContext?.Request.IsHttps ?? false;

                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(30),
                    HttpOnly = false, // Must be false for JS to read it if needed
                    // --- FIXED: Only set Secure if we are actually using HTTPS ---
                    Secure = isSecure,
                    // -------------------------------------------------------------
                    IsEssential = true,
                    SameSite = SameSiteMode.Lax
                };

                // We use checking to prevent crashing if Response is not available
                if (_httpContextAccessor.HttpContext != null)
                {
                    _httpContextAccessor.HttpContext.Response.Cookies.Append(ReferralCookieKey, refCode, cookieOptions);
                }
            }
        }
    }
}