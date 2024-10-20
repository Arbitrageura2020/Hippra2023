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
    public class FTRefreshSignInModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;


        public FTRefreshSignInModel(SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;

        }
        //public async Task<IActionResult> OnGet()
        //{

        //    if (_signInManager.IsSignedIn(User))
        //    {
        //        var user = await _userManager.GetUserAsync(User);
        //        await _signInManager.RefreshSignInAsync(user);
        //    }

        //    return Redirect("~/");
        //}
        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                await _signInManager.RefreshSignInAsync(user);
            }

            return Redirect(returnUrl);
        }
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                await _signInManager.RefreshSignInAsync(user);
            }

            return Redirect(returnUrl);
        }
    }
}
