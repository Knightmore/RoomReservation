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
// Created: 07.10.2022
// Modified: 16.10.2022

#endregion

namespace RoomReservation.Models.AccountViewModels;

public class RegisterViewModel
{
    [Required]
    [RegularExpression(@"^[a-zA-Z-]+$", ErrorMessage = "Only letters and hyphens allowed.")]
    public string Firstname { get; set; }

    [Required]
    [RegularExpression(@"^[a-zA-Z-]+$", ErrorMessage = "Only letters and hyphens allowed.")]
    public string Lastname { get; set; }

    [Required]
    [EmailAddress]
    [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "No valid email address.")]
    public string Email { get; set; }

    [Required] public string Password { get; set; }

    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string PasswordConfirm { get; set; }
}