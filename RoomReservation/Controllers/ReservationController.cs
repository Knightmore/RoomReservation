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

using System.Globalization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RoomReservation.Controllers;

public class ReservationController : DatabaseController
{
    private readonly IConfiguration       _configuration;
    private readonly UserManager<AppUser> _userManager;

    public ReservationController(UserManager<AppUser> userManager, IConfiguration configuration)
    {
        _userManager   = userManager;
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        return Redirect("~/");
    }

    [HttpPost]
    [Authorize]
    public async Task<string> FetchEvents(string start, string end)
    {
        // TODO: Join Reservations with AspNetUsers to get rid of redundant "Title" column. Get "Title" instead through Lastname and Firstname (see Reservation.cs)
        List<Reservation> reservationRange = await DbContext.Reservations.Where(x => x.Start >= Convert.ToDateTime(start, CultureInfo.InvariantCulture) && x.Start <= Convert.ToDateTime(end, CultureInfo.InvariantCulture))
                                                            .ToListAsync();
        return new JArray(reservationRange.Select(x => new JObject
                                                       {
                                                           { "id", $"{x.Start:yyyy-MM-dd}|{x.ResourceId}" },
                                                           { "resourceId", x.ResourceId },
                                                           { "title", x.Title },
                                                           { "start", x.Start },
                                                           { "allDay", true },
                                                           { "backgroundColor", x.UserId == "0" ? "#ff0000" : x.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier) ? "#ccffcc" : "#cce5ff" },
                                                           {
                                                               "display", x.UserId == "0"
                                                                              ? "background"
                                                                              : "auto"
                                                           },
                                                           { "extendedProps", new JObject { { "date", x.Start }, { "userId", x.UserId } } }
                                                       })).ToString(Formatting.None);
    }

    [HttpPost]
    [Authorize]
    public async Task<string> FetchRooms()
    {
        return JsonConvert.SerializeObject(await DbContext.Rooms.Join(DbContext.Seats, room => room.RoomId, seat => seat.RoomId, (room, seat) => new JObject { { "id", seat.id }, { "title", seat.title }, { "Name", room.Name }, { "extendedProps", new JObject { { "extraInfo", seat.ExtraInfo } } } })
                                                          .ToListAsync());
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<string> FetchPersonsForToday()
    {
        List<Reservation> reservations = await DbContext.Reservations.Where(x => x.Start == DateTime.Today)
                                                        .ToListAsync();
        IEnumerable<string> persons = _userManager.Users.ToList()
                                                  .Join(reservations, user => user.Id, reservation => reservation.UserId, (user, _) => new string($"{user.LastName}, {user.FirstName}"));
        return string.Join(":", persons);
    }

    [HttpPost]
    [Authorize]
    public async Task<bool> AddOrDeleteOnDateClick(string resourceId, string start)
    {
        if (string.IsNullOrEmpty(resourceId) || string.IsNullOrEmpty(start)) return false;
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Convert.ToBoolean(_configuration.GetSection("ReservationSettings")["AllowMultiplePerDay"]))
            if (await DbContext.Reservations.AnyAsync(x => x.Start == Convert.ToDateTime(start, CultureInfo.InvariantCulture) && x.UserId == userId))
                return false;

        IQueryable<Reservation> reservations = DbContext.Reservations.Where(x => x.Start == Convert.ToDateTime(start, CultureInfo.InvariantCulture) && x.ResourceId == resourceId);
        if (!reservations.Any())
        {
            AppUser? user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null) return false;
            await DbContext.Reservations.AddAsync(new Reservation
                                                  {
                                                      ResourceId = resourceId,
                                                      Start      = Convert.ToDateTime(start, CultureInfo.InvariantCulture),
                                                      Title      = $"{user.UserName}",
                                                      UserId     = userId,
                                                      Uid = Guid.NewGuid()
                                                                .ToString()
                                                  });
            await DbContext.SaveChangesAsync();

            if (Convert.ToBoolean(_configuration.GetSection("RoomSettings")["UseReservationLimit"]))
            {
                IQueryable<Reservation> reservationRange = DbContext.Reservations.Where(x => x.Start == Convert.ToDateTime(start, CultureInfo.InvariantCulture));
                int                     roomID           = (await DbContext.Seats.FirstOrDefaultAsync(x => x.id     == resourceId))!.RoomId;
                int                     roomLimit        = (await DbContext.Rooms.FirstOrDefaultAsync(x => x.RoomId == roomID))!.Limit;
                var seatsInRoom = await reservationRange.Join(DbContext.Seats, reservation => reservation.ResourceId, seat => seat.id, (reservation, seat) => new { Seat = reservation.ResourceId, Room = seat.RoomId })
                                                        .Where(roomseats => roomseats.Room == roomID)
                                                        .ToListAsync();

                if (seatsInRoom.Count == roomLimit)
                    foreach (Seat seat in DbContext.Seats.Where(seat => seat.RoomId == roomID))
                        if (!seatsInRoom.Any(x => x.Seat == seat.id))
                            await DbContext.Reservations.AddAsync(new Reservation
                                                                  {
                                                                      Start      = Convert.ToDateTime(start, CultureInfo.InvariantCulture),
                                                                      ResourceId = seat.id,
                                                                      Title      = "",
                                                                      UserId     = "0",
                                                                      Uid        = "00000000-0000-0000-0000-000000000000"
                                                                  });

                await DbContext.SaveChangesAsync();
            }

            return true;
        }

        return false;
    }

    [HttpPost]
    [Authorize]
    public async Task<bool> DeleteReservation(string reservationId)
    {
        var          start       = Convert.ToDateTime(reservationId.Split('|')[0]);
        string       resourceId  = reservationId.Split('|')[1];
        Reservation? reservation = await DbContext.Reservations.FirstOrDefaultAsync(x => x.Start == start && x.ResourceId == resourceId);
        if (reservation == null) return false;
        if (reservation.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier) && !User.IsInRole("Admin")) return false;
        DbContext.Reservations.Remove(reservation);
        await DbContext.SaveChangesAsync();
        if (Convert.ToBoolean(_configuration.GetSection("RoomSettings")["UseReservationLimit"]))
        {
            IQueryable<Reservation> reservationRange = DbContext.Reservations.Where(x => x.Start == Convert.ToDateTime(start));
            int                     roomID           = (await DbContext.Seats.FirstOrDefaultAsync(x => x.id     == resourceId))!.RoomId;
            int                     roomLimit        = (await DbContext.Rooms.FirstOrDefaultAsync(x => x.RoomId == roomID))!.Limit;
            var seatsInRoom = await reservationRange.Join(DbContext.Seats, reservation => reservation.ResourceId, seat => seat.id, (reservation, seat) => new { reservation.Start, reservation.ResourceId, seat.RoomId, reservation.UserId })
                                                    .Where(roomSeats => roomSeats.RoomId == roomID)
                                                    .ToListAsync();
            if (seatsInRoom.Count(seat => seat.UserId != "0") < roomLimit)
            {
                List<Reservation> reservationsToDelete = new();
                Reservation?      tempReservation;
                foreach (var seat in seatsInRoom.Where(seat => seat.UserId == "0"))
                {
                    tempReservation = await DbContext.Reservations.FirstOrDefaultAsync(x => x.Start == seat.Start && x.ResourceId == seat.ResourceId);
                    if (tempReservation != null)
                        reservationsToDelete.Add(tempReservation);
                }

                DbContext.Reservations.RemoveRange(reservationsToDelete);
                await DbContext.SaveChangesAsync();
            }
        }

        return true;
    }
}