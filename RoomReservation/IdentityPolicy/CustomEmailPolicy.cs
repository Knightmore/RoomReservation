#region Copyright © 2023 Patrick Borger - https: //github.com/Knightmore

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
// Modified: 19.01.2023

#endregion

namespace RoomReservation.IdentityPolicy;

public class CustomEmailPolicy : UserValidator<AppUser>
{
    public override async Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
    {
        IConfigurationRoot? configuration = new ConfigurationBuilder().AddJsonFile("customsettings.json")
                                                                      .Build();
        string?        mailConfig   = configuration.GetSection("AccountSettings")["AllowedMails"];
        string[]       allowedMails = mailConfig.Split(",");
        IdentityResult result       = await base.ValidateAsync(manager, user);
        List<IdentityError> errors = result.Succeeded
                                         ? new List<IdentityError>()
                                         : result.Errors.ToList();

        if (!allowedMails.Any(x => user.Email.ToLower()
                                       .EndsWith(x.ToLower())))
            errors.Add(new IdentityError { Description = "Your email address provider is not allowed." });
        return errors.Count == 0
                   ? IdentityResult.Success
                   : IdentityResult.Failed(errors.ToArray());
    }
}