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
// Modified: 07.11.2022

#endregion

using Microsoft.Extensions.Localization;
using RoomReservation.Models;
using RoomReservation.Models.AccountViewModels;
using System.Text.Encodings.Web;

namespace RoomReservation.Controllers;

public class UserManagementController : Controller
{
    private readonly IPasswordHasher<AppUser> _passwordHasher;

    private readonly IPasswordValidator<AppUser> _passwordValidator;

    private readonly RoleManager<IdentityRole> _roleManager;

    private readonly UserManager<AppUser> _userManager;

    private readonly IUserValidator<AppUser>                    _userValidator;
    private readonly IStringLocalizer<UserManagementController> _localizer;

    public UserManagementController(UserManager<AppUser> usrManager, IPasswordHasher<AppUser> passwordHasher, IPasswordValidator<AppUser> passwordValidator, IUserValidator<AppUser> userValidator, RoleManager<IdentityRole> roleManager, IStringLocalizer<UserManagementController> localizer)
    {
        _userManager       = usrManager;
        _passwordHasher    = passwordHasher;
        _passwordValidator = passwordValidator;
        _userValidator     = userValidator;
        _roleManager       = roleManager;
        _localizer         = localizer;
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Index()
    {
        return View(_userManager.Users.Where(user => user.Id != "0").OrderBy(user => user.LastName));
    }

    [Authorize(Roles = "Admin")]
    public ViewResult Create()
    {
        return View();
    }

    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> Create(AppUserViewModel user)
    {
        if (!ModelState.IsValid) return View(user);
        var appUser = new AppUser
                      {
                          UserName       = $"{user.Lastname}, {user.Firstname}",
                          FirstName      = user.Firstname,
                          LastName       = user.Lastname,
                          Email          = user.Email,
                          Role           = _roleManager.FindByNameAsync("Member").Result.Id,
                          EmailConfirmed = false,
                          ICalGuid       = Guid.NewGuid().ToString()
                      };

        IdentityResult result = await _userManager.CreateAsync(appUser, user.Password);

        if (result.Succeeded)
        {
            result = await _userManager.AddToRoleAsync(appUser, "Member");
            if (result.Succeeded)
            {
                TempData["success"] = HtmlEncoder.Default.Encode(_localizer["User {0}, {1} successfully created!", appUser.LastName, appUser.FirstName].ToString());
                return RedirectToAction("Index");
            }
        }

        foreach (IdentityError error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return View(user);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(string id)
    {
        AppUser user = await _userManager.FindByIdAsync(id);
        if (user != null) return View(user);

        return RedirectToAction("Index");
    }

    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> Update(string id, string firstname, string lastname, string email, bool emailConfirmed, bool lockedOut, string role, string? password)
    {
        AppUser user = await _userManager.FindByIdAsync(id);
        if (user == null) return View(user);

        if (!string.IsNullOrEmpty(firstname)) user.FirstName = firstname;
        if (!string.IsNullOrEmpty(lastname)) user.LastName   = lastname;
        IdentityResult validEmail                            = default!;
        if (!string.IsNullOrEmpty(email))
        {
            user.Email = email;
            validEmail = await _userValidator.ValidateAsync(_userManager, user);
            if (!validEmail.Succeeded)
                Errors(validEmail);
        }

        if (!string.IsNullOrEmpty(password))
        {
            IdentityResult validPass                   = await _passwordValidator.ValidateAsync(_userManager, user, password);
            if (validPass.Succeeded) user.PasswordHash = _passwordHasher.HashPassword(user, password);

            Errors(validPass);
        }

        if (!validEmail.Succeeded)
            return View(user);

        if (user.Id == "2f697355-9779-47ee-b278-c97cbee59020" && (role != "d83cc7ab-bb04-44aa-bf91-1e17c1b45fcb" || emailConfirmed == false || lockedOut))
        {
            ModelState.AddModelError(string.Empty, _localizer["Integrated safety: You can't remove the Admin role or change the lockout status for Super Admin!"]);
        }
        else
        {
            user.EmailConfirmed = emailConfirmed;
            user.LockedOut      = lockedOut;
            user.LockoutEnd     = lockedOut ? DateTimeOffset.MaxValue : DateTimeOffset.UtcNow.AddSeconds(-1);

            IdentityResult removedRoles = await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));
            if (removedRoles.Succeeded)
            {
                IdentityResult addedToRole = await _userManager.AddToRoleAsync(user, _roleManager.FindByIdAsync(role).Result.Name);
                if (addedToRole.Succeeded)
                {
                    user.Role = role;
                }
                else
                {
                    Errors(addedToRole);
                    return View(user);
                }
            }
            else
            {
                Errors(removedRoles);
                return View(user);
            }
        }

        IdentityResult result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            TempData["success"] = HtmlEncoder.Default.Encode(_localizer["User {0}, {1} successfully updated!", user.LastName, user.FirstName].ToString());
            return View(user);
        }

        Errors(result);

        return View(user);
    }

    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> Delete(string id)
    {
        AppUser user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            IdentityResult result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["success"] = HtmlEncoder.Default.Encode(_localizer["User {0}, {1} successfully deleted!", user.LastName, user.FirstName].ToString());
                return RedirectToAction("Index");
            }

            Errors(result);
        }
        else
        {
            ModelState.AddModelError(string.Empty, _localizer["User {0}, {1} not found!", user.LastName, user.FirstName]);
        }

        return View("Index", _userManager.Users);
    }

    private void Errors(IdentityResult result)
    {
        foreach (IdentityError error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);
    }
}