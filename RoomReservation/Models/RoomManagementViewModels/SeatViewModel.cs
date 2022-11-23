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
// Created: 10.10.2022
// Modified: 20.10.2022

#endregion

namespace RoomReservation.Models;

public class SeatViewModel
{
    public string  Id        { get; set; }
    public string  Title     { get; set; }
    public string? ExtraInfo { get; set; }
    public int     RoomId    { get; set; }
    public string  RoomName  { get; set; }
    public int     RoomLimit { get; set; }
}