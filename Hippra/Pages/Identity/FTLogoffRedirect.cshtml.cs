using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Hippra.Models.SQL;

namespace Hippra.Pages.FTDesign.Pages.Identity
{
    public class FTLogoffRedirectModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;


        public FTLogoffRedirectModel(SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;

        }

        public async Task<IActionResult> OnGet()
        {
            if (_signInManager.IsSignedIn(User))
            {
                await _signInManager.SignOutAsync();
            }

            return Redirect("~/FTRZRelogin");
        }
    }
}
