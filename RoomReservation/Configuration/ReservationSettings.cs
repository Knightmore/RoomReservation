﻿#region Copyright © 2023 Patrick Borger - https: //github.com/Knightmore

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
// Created: 17.01.2023
// Modified: 19.01.2023

#endregion

namespace RoomReservation.Configuration;

public class ReservationSettings
{
    public bool            AllowMultiplePerDay;
    public bool            AutomaticRefresh;
    public bool            AutomaticRefreshAdminsOnly;
    public bool            DayHeaders;
    public bool            ExpandRows;
    public string?         Height;
    public string?         InitialView;
    public int             RefreshAfterInMs;
    public bool            ResourcesInitiallyExpanded;
    public SlotLabelFormat SlotLabelFormat;
    public string?         TimeZone;
    public bool            Weekends;
}

public class SlotLabelFormat
{
    public string? Day;
    public string? Month;
    public string? Weekday;
    public string? Year;
}