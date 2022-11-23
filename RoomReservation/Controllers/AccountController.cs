#region Copyright © 2022 Patrick Borger - https: //github.com/Knightmore

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 
// Author: Patrick Borger
// GitHub: https://github.com/Knightmore
// Created: 07.10.2022
// Modified: 17.10.2022

#endregion

using Microsoft.Extensions.Localization;
using RoomReservation.Models.AccountViewModels;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace RoomReservation.Controllers;

[Authorize]
public class AccountController : DatabaseController
{
    private readonly IStringLocalizer<AccountController> _localizer;
    private readonly SignInManager<AppUser>              _signInManager;
    private readonly UserManager<AppUser>                _userManager;

    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signinManager, IStringLocalizer<AccountController> localizer)
    {
        _userManager   = userManager;
        _signInManager = signinManager;
        _localizer     = localizer;
    }

    [AllowAnonymous]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel login)
    {
        if (!ModelState.IsValid) return View(login);
        AppUser appUser = await _userManager.FindByEmailAsync(login.Email);
        if (appUser != null)
        {
            if (appUser.EmailConfirmed)
            {
                await _signInManager.SignOutAsync();
                SignInResult result = await _signInManager.PasswordSignInAsync(appUser, login.Password, login.Remember, true);

                if (result.IsLockedOut)
                {
                    appUser.LockedOut = true;
                    _                 = await _userManager.UpdateAsync(appUser);
                    ModelState.AddModelError(string.Empty, _localizer["The account is locked out. Please try again later."]);
                    return View(login);
                }

                if (result.Succeeded)
                {
                    appUser.LockedOut = false;
                    _                 = await _userManager.UpdateAsync(appUser);
                    return Redirect("~/");
                }
            }
            else
            {
                ModelState.AddModelError(nameof(login.Email), _localizer["Account not confirmed."]);
                return View(login);
            }
        }

        ModelState.AddModelError(string.Empty, _localizer["Login failed: Email or password wrong!"]);

        return View(login);
    }

    [AllowAnonymous]
    public IActionResult Register()
    {
        if (User.Identity is { IsAuthenticated: false })
            return View();
        return Redirect("~/");
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel viewModel)
    {
        if (!ModelState.IsValid) return View(viewModel);

        var newUser = new AppUser
                      {
                          UserName       = $"{viewModel.Lastname}, {viewModel.Firstname}",
                          FirstName      = viewModel.Firstname,
                          LastName       = viewModel.Lastname,
                          Email          = viewModel.Email,
                          Role           = "Member",
                          EmailConfirmed = false,
                          ICalGuid       = Guid.NewGuid().ToString()
                      };
        IdentityResult result = await _userManager.CreateAsync(newUser, viewModel.Password);

        if (result.Succeeded)
        {
            result = await _userManager.AddToRoleAsync(newUser, "Member");
            if (result.Succeeded)
            {
                TempData["success"] = _localizer["Successfully created user {0}, {1}.", viewModel.Lastname, viewModel.Firstname].ToString();
                return Redirect("~/");
            }

            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return View(viewModel);
        }

        foreach (IdentityError error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return View(viewModel);
    }

    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> ChangePassword(PasswordViewModel viewModel)
    {
        if (!ModelState.IsValid) return View();

        AppUser? user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound(_localizer["Unable to load user with ID '{0}'.", _userManager.GetUserId(User)]);

        IdentityResult changePasswordResult = await _userManager.ChangePasswordAsync(user, viewModel.Password, viewModel.NewPassword);
        if (!changePasswordResult.Succeeded)
        {
            foreach (IdentityError error in changePasswordResult.Errors) ModelState.AddModelError(string.Empty, error.Description);
            return View();
        }

        await _signInManager.RefreshSignInAsync(user);

        return Redirect("~/");
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}