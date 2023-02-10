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
// Created: 07.11.2022
// Modified: 19.01.2023

#endregion

using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;

namespace RoomReservation.Controllers;

[Route("[controller]")]
[ApiController]
public class ICalFeedController : DatabaseController
{
    private readonly IConfiguration       _configuration;
    private readonly UserManager<AppUser> _userManager;

    public ICalFeedController(UserManager<AppUser> userManager, IConfiguration configuration)
    {
        _userManager   = userManager;
        _configuration = configuration;
    }

    [AllowAnonymous]
    [HttpGet("{icalguid}")]
    public ActionResult<string> Index(string icalguid)
    {
        var calendar = new Calendar();
        if (icalguid == _configuration.GetSection("iCalSettings")["GUIDForAll"])
        {
            var reservations = DbContext.Reservations.Join(DbContext.Seats, reservation => reservation.ResourceId, seat => seat.id, (reservation, seat) => new { reservation, seat })
                                        .Join(DbContext.Rooms, rs => rs.seat.RoomId, room => room.RoomId, (rs,                                    room) => new { rs, room })
                                        .Join(_userManager.Users, rsr => rsr.rs.reservation.UserId, user => user.Id, (rsr, user) => new
                                                                                                                                    {
                                                                                                                                        rsr.rs.reservation.Start,
                                                                                                                                        rsr.rs.reservation.Uid,
                                                                                                                                        Room     = rsr.room.Name,
                                                                                                                                        Seat     = rsr.rs.seat.title,
                                                                                                                                        Username = $"{user.LastName}, {user.FirstName}"
                                                                                                                                    });

            foreach (var reservation in reservations)
            {
                if (reservation.Uid == "00000000-0000-0000-0000-000000000000") continue;
                var e = new CalendarEvent
                        {
                            Start    = new CalDateTime(reservation.Start),
                            End      = new CalDateTime(reservation.Start.AddDays(1)),
                            Uid      = reservation.Uid,
                            Summary  = reservation.Username,
                            Location = $"{reservation.Room} - {reservation.Seat}"
                        };
                e.AddProperty("X-MICROSOFT-CDO-BUSYSTATUS", "WORKINGELSEWHERE");
                calendar.Events.Add(e);
            }
        }
        else
        {
            AppUser? user = _userManager.Users.FirstOrDefault(x => x.ICalGuid == icalguid);
            if (user == null) return string.Empty;
            var reservations = DbContext.Reservations.Where(x => x.UserId == user.Id)
                                        .Join(DbContext.Seats, reservation => reservation.ResourceId, seat => seat.id, (reservation, seat) => new { reservation, seat })
                                        .Join(DbContext.Rooms, rs => rs.seat.RoomId, room => room.RoomId, (rs,                       room) => new { rs.reservation.Start, rs.reservation.Uid, Room = room.Name, Seat = rs.seat.title });

            foreach (var reservation in reservations)
            {
                var e = new CalendarEvent { Start = new CalDateTime(reservation.Start), End = new CalDateTime(reservation.Start.AddDays(1)), Uid = reservation.Uid, Summary = $"{reservation.Room} - {reservation.Seat}" };
                e.AddProperty("X-MICROSOFT-CDO-BUSYSTATUS", "WORKINGELSEWHERE");
                calendar.Events.Add(e);
            }
        }


        var serializer = new CalendarSerializer();
        return serializer.SerializeToString(calendar);
    }
}