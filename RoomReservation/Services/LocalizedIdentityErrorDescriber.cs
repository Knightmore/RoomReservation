﻿#region Copyright © 2022 Patrick Borger - https: //github.com/Knightmore

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
// Created: 09.11.2022
// Modified: 09.11.2022

#endregion

using Microsoft.Extensions.Localization;

namespace RoomReservation.Services;

public class LocalizedIdentityErrorDescriber : IdentityErrorDescriber
{
    private readonly IStringLocalizer<LocalizedIdentityErrorDescriber> _localizer;

    public LocalizedIdentityErrorDescriber(IStringLocalizer<LocalizedIdentityErrorDescriber> localizer)
    {
        _localizer = localizer;
    }

    public override IdentityError DuplicateEmail(string email)
    {
        return new IdentityError { Code = nameof(DuplicateEmail), Description = string.Format(_localizer["Email {0} is already taken."], email) };
    }

    public override IdentityError DefaultError()
    {
        return new IdentityError { Code = nameof(DefaultError), Description = _localizer["An unknown failure has occurred."] };
    }

    public override IdentityError ConcurrencyFailure()
    {
        return new IdentityError { Code = nameof(ConcurrencyFailure), Description = _localizer["Optimistic concurrency failure, object has been modified."] };
    }

    public override IdentityError PasswordMismatch()
    {
        return new IdentityError { Code = nameof(PasswordMismatch), Description = _localizer["Incorrect password."] };
    }

    public override IdentityError InvalidToken()
    {
        return new IdentityError { Code = nameof(InvalidToken), Description = _localizer["Invalid token."] };
    }

    public override IdentityError LoginAlreadyAssociated()
    {
        return new IdentityError { Code = nameof(LoginAlreadyAssociated), Description = _localizer["A user with this login already exists."] };
    }

    public override IdentityError InvalidUserName(string userName)
    {
        return new IdentityError { Code = nameof(InvalidUserName), Description = string.Format(_localizer["User name {0} is invalid, can only contain letters or digits."], userName) };
    }

    public override IdentityError InvalidEmail(string email)
    {
        return new IdentityError { Code = nameof(InvalidEmail), Description = string.Format(_localizer["Email {0} is invalid."], email) };
    }

    public override IdentityError DuplicateUserName(string userName)
    {
        return new IdentityError { Code = nameof(DuplicateUserName), Description = string.Format(_localizer["User Name {0} is already taken."], userName) };
    }

    public override IdentityError InvalidRoleName(string role)
    {
        return new IdentityError { Code = nameof(InvalidRoleName), Description = string.Format(_localizer["Role name {0} is invalid."], role) };
    }

    public override IdentityError DuplicateRoleName(string role)
    {
        return new IdentityError { Code = nameof(DuplicateRoleName), Description = string.Format(_localizer["Role name {0} is already taken."], role) };
    }

    public override IdentityError UserAlreadyHasPassword()
    {
        return new IdentityError { Code = nameof(UserAlreadyHasPassword), Description = _localizer["User already has a password set."] };
    }

    public override IdentityError UserLockoutNotEnabled()
    {
        return new IdentityError { Code = nameof(UserLockoutNotEnabled), Description = _localizer["Lockout is not enabled for this user."] };
    }

    public override IdentityError UserAlreadyInRole(string role)
    {
        return new IdentityError { Code = nameof(UserAlreadyInRole), Description = string.Format(_localizer["User already in role {0}."], role) };
    }

    public override IdentityError UserNotInRole(string role)
    {
        return new IdentityError { Code = nameof(UserNotInRole), Description = string.Format(_localizer["User is not in role {0}."], role) };
    }

    public override IdentityError PasswordTooShort(int length)
    {
        return new IdentityError { Code = nameof(PasswordTooShort), Description = string.Format(_localizer["Passwords must be at least {0} characters."], length) };
    }

    public override IdentityError PasswordRequiresNonAlphanumeric()
    {
        return new IdentityError { Code = nameof(PasswordRequiresNonAlphanumeric), Description = _localizer["Passwords must have at least one non alphanumeric character."] };
    }

    public override IdentityError PasswordRequiresDigit()
    {
        return new IdentityError { Code = nameof(PasswordRequiresDigit), Description = _localizer["Passwords must have at least one digit ('0'-'9')."] };
    }

    public override IdentityError PasswordRequiresLower()
    {
        return new IdentityError { Code = nameof(PasswordRequiresLower), Description = _localizer["Passwords must have at least one lowercase ('a'-'z')."] };
    }

    public override IdentityError PasswordRequiresUpper()
    {
        return new IdentityError { Code = nameof(PasswordRequiresUpper), Description = _localizer["Passwords must have at least one uppercase ('A'-'Z')."] };
    }
    // DuplicateUserName, InvalidEmail, DuplicateUserName etc
}