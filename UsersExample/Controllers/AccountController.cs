﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UsersExample.Models;
using Microsoft.AspNetCore.Identity;

namespace UsersExample.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private UserManager<AppUser> userManager;
        private SignInManager<AppUser> signInManager;

        public AccountController(UserManager<AppUser> userMgr, SignInManager<AppUser> signinMgr)
        {
            userManager = userMgr;
            signInManager = signinMgr;
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel details, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await userManager.FindByEmailAsync(details.Email);
                if (user != null)
                {
                    await signInManager.SignOutAsync();
                    Microsoft.AspNetCore.Identity.SignInResult result =
                        await signInManager.PasswordSignInAsync(
                            user, details.Password, false, false);
                    if (result.Succeeded)
                    {
                        return Redirect(returnUrl ?? "/");
                    }
                }
                ModelState.AddModelError(nameof(LoginModel.Email), "Invalid user or password");
            }
            return View(details);
        }
    }
}
